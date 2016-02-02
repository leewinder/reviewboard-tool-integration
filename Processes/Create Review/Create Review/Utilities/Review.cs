using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace Create_Review.Utilities
{
    // Declares the properties defined in the review request
    public class Review
    {
        // Level of review possible
        public enum Level
        {
            FullReview,
            PendingReview,
            NoReview
        };

        // Source Types
        public enum Source
        {
            Files,
            Patch,
            None,
        }

        //
        // Content of the review passed to the review
        //
        public class Content
        {
            public readonly Source   Source;
            public readonly string   Patch;

            public readonly string[] Files;

            // Constructor
            public Content(Source source, string patch, string[] files)
            {
                Source = source;
                Patch = patch;
                Files = files;
            }
        };

        //
        // Properties of the current review
        //
        public class Properties
        {
            public readonly string       Path;
            public readonly Content      Contents;

            public readonly string       ReviewId;

            public readonly string       Summary;
            public readonly string       Description;
            public readonly string       Testing;

            public readonly string       JiraId;

            public readonly List<string> Groups;

            public readonly Level        ReviewLevel;
            public readonly bool         CopiesAsAdds;

            public Properties(string path, Content reviewProperties, string reviewId, string summary, string description, string testing, string jiraId, List<string> groups, Level level, bool copiesAsAdds)
            {
                Path = path;

                Contents = reviewProperties;

                ReviewId = reviewId;

                Summary = summary;
                Description = description;
                Testing = testing;

                JiraId = jiraId;

                Groups = groups;

                ReviewLevel = level;
                CopiesAsAdds = copiesAsAdds;
            }
        };

        //
        // Extracts the content of the review from the file passed to the tool
        //
        public static Content ExtractContent(string requestSource, string injectRequest)
        {
            // Read in the request source
            string[] modifiedContent = ReadRequestSource(requestSource);
            if (modifiedContent == null)
                return null;

            // Inject the working copy into the requests if we've been asked to do it
            modifiedContent = InjectWorkingDirectory(modifiedContent, injectRequest);

            // Check to see if we have a review with a single patch file
            Content reviewContent = CreatePatchOnlyReview(modifiedContent);
            if (reviewContent != null)
                return reviewContent;

            // Calculate the root path of this review
            string rootPath = FindReviewRootPath(modifiedContent);
            if (rootPath == null)
                return null;
            
            // Reject any invalid files
            modifiedContent = RejectInvalidFiles(modifiedContent, rootPath);
            if (modifiedContent == null)
                return null;
            
            // Before we modify the files to allow the generation of the diff, we need to 
            // keep track of the absolute file names we're going to push to SVN
            string[] filesToSubmit = modifiedContent.ToArray();

            // Now make the review relative to the root directory
            modifiedContent = MoveContentRelativeToRoot(modifiedContent, rootPath);

            // Create the final patch file for the review
            string patchFileName = CreateReviewPatch(modifiedContent, rootPath);
            if (patchFileName == null)
                return null;

            // Return our patch file
            return new Content(Source.Files, patchFileName, filesToSubmit);
        }

        // Private Properties
        private static readonly string[]    ValidPatchExtensions = { ".patch", ".diff" };
        private static readonly string      InjectDirectoryRequest = "inject_test_path";

        //
        // Reads in and checks the content
        //
        private static string[] ReadRequestSource(string requestSource)
        {
            // Read in the request source
            string[] fileContent = null;
            try
            {
                fileContent = File.ReadAllLines(requestSource);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to open the review request data file\n\n" + e.Message, "Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }


            // We need some entries so strip out white space and check
            var reviewContent = fileContent.Where(x => string.IsNullOrWhiteSpace(x) == false);
            if (reviewContent.Count() == 0)
            {
                MessageBox.Show("No content was found that could actually be reviewed", "Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Return the content
            return reviewContent.ToArray();
        }

        //
        // Returns if we want to inject our working directory into the file requests
        //
        private static string[] InjectWorkingDirectory(string[] reviewContent, string injectRequest)
        {
            // Do we want to inject the path of the files because we're running a test
            bool injectTestPath = false;
            if (string.IsNullOrWhiteSpace(injectRequest) == false)
            {
                if (injectRequest.Equals(InjectDirectoryRequest, StringComparison.InvariantCultureIgnoreCase) == true)
                    injectTestPath = true;
            }

            // If we need to, inject the test path
            if (injectTestPath == true)
            {
                // Get the path we need to inject into the test documents
                string runningPath = Directory.GetCurrentDirectory();

                // Removed the working copy
                string injectionPath = System.IO.Path.GetDirectoryName(runningPath);
                injectionPath = System.IO.Path.GetDirectoryName(injectionPath);
                injectionPath = Paths.Clean(injectionPath);

                // Spin through the entries and inject the directory so our paths are relative
                reviewContent = reviewContent.Select(x =>
                {
                    // Remove the root and clean up
                    x = string.Format(x, injectionPath);
                    x = Paths.Clean(x);

                    return x;
                }).ToArray();
            }

            // Return our content
            return reviewContent;
        }

        //
        // Create a review if the content only contains a single patch file
        //
        private static Content CreatePatchOnlyReview(string[] reviewContent)
        {
            // If we have more than one file, it can't be a patch file
            if (reviewContent.Length != 1)
                return null;

            // Get the file, is it a patch or diff file?
            string fileToCheck = reviewContent[0];
            if (IsFilePatch(fileToCheck) == false)
                return null;

            // It is a patch file, now we need to make sure it's not part of the 
            // repository because otherwse we'd need to review that!
            if (Svn.IsPathTracked(fileToCheck) == true)
                return null;

            // This is a patch file that has been selected specifically to review the 
            // content and is not part of the repository
            return new Content(Source.Patch, fileToCheck, null);
        }

        //
        // Finds the root path of the review
        //
        private static string FindReviewRootPath(string[] reviewContent)
        {
            // Spin through and get the root
            string rootPath = string.Empty;
            foreach (string thisEntry in reviewContent)
            {
                // Get the root of this file
                string thisRoot = Svn.GetRoot(thisEntry);
                if (thisRoot == null)
                    continue;

                // We have a root, send it back
                return thisRoot;
            }

            // If we got here, no root
            MessageBox.Show("Unable to find a working SVN root directly", "Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Return nothing
            return null;
        }

        //
        // Rejects the files in the list that are not valid
        //
        private static string[] RejectInvalidFiles(string[] reviewContent, string rootPath)
        {
            // Create a list to keep track of rejected files
            List<string> rejectedFiles = new List<string>();

            // Check all the entries to review are valid
            reviewContent = reviewContent.Where(x =>
            {
                // Get the root path in which this repostory lives
                string thisRoot = Svn.GetRoot(x);

                // If we don't have a root it's not source controlled so bail
                if (thisRoot == null)
                {
                    rejectedFiles.Add(x);
                    return false;
                }

                // If we're under the same root we're fine
                bool underSameRoot = rootPath.Equals(thisRoot);
                if (underSameRoot == false)
                {
                    rejectedFiles.Add(x);
                    return false;
                }

                // Return that we should keep this file
                return true;
            }).ToArray();

            // If we have any rejected files, let the user know
            if (rejectedFiles.Count != 0)
            {
                StringBuilder errorMessage = new StringBuilder("Unable to include the following files for review\n");
                foreach (string thisError in rejectedFiles)
                {
                    string stringToShow = Utilities.Paths.TruncateLongPath(thisError);
                    errorMessage.Append("- " + stringToShow + "\n");
                }

                MessageBox.Show(errorMessage.ToString(), "Warning when raising review", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Do we have any files left?
            if (reviewContent.Length == 0)
                MessageBox.Show("No content was found that could actually be reviewed", "Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Return the updated files
            return reviewContent.Count() == 0 ? null : reviewContent;
        }

        //
        // Removes the root path from the entries so the diff is relative
        //
        private static string[] MoveContentRelativeToRoot(string[] reviewContent, string rootPath)
        {
            // Remove the root from every entry
            var modifiedContent = reviewContent.Select(x =>
            {
                // Remove the root and clean up
                x = x.Remove(0, rootPath.Length);
                x = Paths.Clean(x);

                return x;
            });
            reviewContent = modifiedContent.Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray();

            // Return the results
            return reviewContent;
        }

        //
        // Creates the patch file containing the files we want to review
        //
        private static string CreateReviewPatch(string[] reviewContent, string rootPath)
        {
            // Now we need to generate a patch file for all the files we do want to include
            StringBuilder filesToDiff = new StringBuilder("diff ");
            foreach (string thisFile in reviewContent)
                filesToDiff.Append('"').Append(thisFile).Append("\" ");

            // Generate the diff result
            Process.Output diffOutput = Process.Start(rootPath, "svn", filesToDiff.ToString());
            if (string.IsNullOrWhiteSpace(diffOutput.StdErr) == false)
            {
                MessageBox.Show("Unable to generate review patch file\n\n" + diffOutput.StdErr, "Error when raising review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Pipe out the diff to the root directory
            string patchFileName = rootPath + "\\rbi_patch_" + Guid.NewGuid() + ValidPatchExtensions[0];
            File.WriteAllText(patchFileName, diffOutput.StdOut);

            // Return the path file
            return patchFileName;
        }

        //
        // Returns if the file is a patch file
        //
        private static bool IsFilePatch(string file)
        {
            // Needs to exist
            if (File.Exists(file) == false)
                return false;

            // Get the extension of this file
            string extension = System.IO.Path.GetExtension(file);

            // Spin through our valid extensions
            foreach (string thisExtension in ValidPatchExtensions)
            {
                if (extension.Equals(thisExtension, StringComparison.InvariantCultureIgnoreCase) == true)
                    return true;
            }

            // It is not a patch file
            return false;
        }
    };
}
