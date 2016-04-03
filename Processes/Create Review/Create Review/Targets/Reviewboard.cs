using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;

namespace Create_Review
{
    class Reviewboard
    {
        // Returns the results of an authentication request
        public class AuthenticationResult
        {
            public readonly bool    Success;
            public readonly string  Message;

            public readonly string  Password;

            // Constructor
            public AuthenticationResult(bool success, string message, string password)
            {
                Success = success;
                Message = message;

                Password = Utilities.StringCipher.Encrypt(password, Utilities.Identifiers.UUID);
            }
        }

        // Returns the result of a review request
        public class ReviewRequestResult
        {
            public readonly string                      Url;
            public readonly string                      Error;

            public readonly Utilities.Review.Properties Properties;

            // Constructor
            public ReviewRequestResult(string reviewUrl, string reviewError, Utilities.Review.Properties reviewRequest)
            {
                Url = reviewUrl;
                Error = reviewError;
                Properties = reviewRequest;
            }
        };

        // Information about a review group
        public class ReviewGroup
        {
            public readonly string DisplayName;
            public readonly string InternalName;

            // Constructor
            public ReviewGroup(string displayName, string internalName)
            {
                DisplayName = displayName;
                InternalName = internalName;
            }
        }

        //
        // Authenticates reviewboard
        //
        public static AuthenticationResult Authenticate(string server, string username, string password, bool decryptPassword)
        {
            // Check we have the properties we need
            bool validServer = string.IsNullOrWhiteSpace(server) == false;
            bool validUser = string.IsNullOrWhiteSpace(username) == false;
            bool validPassword = string.IsNullOrWhiteSpace(password) == false;

            // If it's not valid, throw and we're done
            if (validServer == false || validUser == false || validPassword == false)
                throw new System.ArgumentException("Invalid server, username or password requested");

            // Update the password if we need to decrypt it
            if (decryptPassword == true)
                password = Utilities.StringCipher.Decrypt(password, Utilities.Identifiers.UUID);

            // Attempt to authenticate with the server
            string commandOptions = string.Format(@"login --server {0} --username {1} --password {2}", server, username, password);
            string rbtPath = GetRbtPath();

            // Run the process
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, rbtPath, commandOptions);
            string errorFlag = "ERROR: ";
            string criticalFlag = "CRITICAL: ";

            // Annoyingly, rbt seems to only post to stderr
            bool succeeded = string.IsNullOrWhiteSpace(output.StdErr) == true;
            if (succeeded == false)
                succeeded = (output.StdErr.StartsWith(errorFlag) == false && output.StdErr.StartsWith(criticalFlag) == false);

            // How did we do?
            string messageToShow = output.StdErr;
            if (succeeded == true)
            {
                messageToShow = @"Successfully authenticated against the Reviewboard server";
            }

            // Return the result
            return new AuthenticationResult(succeeded, messageToShow.Trim(), password);
        }

        //
        // Returns a list of review groups from the given server
        //
        public static ReviewGroup[] GetReviewGroups(string workingDirectory, string server, string username, string password)
        {
            // Build up the command
            string commandOptions = string.Format(@"api-get --server {0} --username {1} --password {2} /groups/", server, username, password);
            string rbtPath = GetRbtPath();

            // Run the process
            Utilities.Process.Output output = Utilities.Process.Start(workingDirectory, rbtPath, commandOptions);

            // Throw to return the error
            if (string.IsNullOrWhiteSpace(output.StdErr) == false)
                throw new ApplicationException(output.StdErr);

            // Get the list we'll add out review groups to
            var returnedGroups = new[]{ new { display_name = "", name = "" } }.ToList();

            // Parse the results of the operation
            JObject parsedJson = JObject.Parse(output.StdOut);
            IList<JToken> results = parsedJson["groups"].Children().ToList();
            
            // Pull out our groups and put them in our list
            foreach(JToken result in results)
            {
                // Add it to the list
                var anonObject = returnedGroups.ElementAt(0);
                var searchResult = JsonConvert.DeserializeAnonymousType(result.ToString(), anonObject);

                returnedGroups.Add(searchResult);
            }

            // Remove the dummy entry
            returnedGroups.RemoveAt(0);

            // Build up the list to return
            ReviewGroup[] groups = new ReviewGroup[returnedGroups.Count];
            for (int i = 0; i < groups.Length; ++i)
            {
                var thisGroup = returnedGroups.ElementAt(i);
                groups[i] = new ReviewGroup(thisGroup.display_name, thisGroup.name);
            }
            
            // Return the list of groups
            return groups;
        }

        //
        // Raises a new review
        //
        public static ReviewRequestResult RequestReview(string workingDirectory, string server, string username, string password, Utilities.Review.Properties reviewProperties)
        {
            // We may not need to generate a review
            if (reviewProperties.ReviewLevel != Utilities.Review.Level.FullReview)
            {
                // No review needed so exit
                return new ReviewRequestResult(string.Empty, string.Empty, reviewProperties);
            }
            
            // Read out the description and testing into a temp file
            string descriptionFile = GetFileWithContents(reviewProperties.Description, reviewProperties.Summary);
            string testingFile = GetFileWithContents(reviewProperties.Testing, null);

            // Build up the review command
            string commandProperties = string.Format("--server {0} --summary \"{1}\" ", server, reviewProperties.Summary);
            if (string.IsNullOrWhiteSpace(descriptionFile) == false)
                commandProperties += string.Format("--description-file \"{0}\" ", descriptionFile);
            if (string.IsNullOrWhiteSpace(testingFile) == false)
                commandProperties += string.Format("--testing-done-file \"{0}\" ", testingFile);
            if (string.IsNullOrWhiteSpace(reviewProperties.JiraId) == false)
                commandProperties += string.Format("--bugs-closed \"{0}\" ", reviewProperties.JiraId);

            // Options
            commandProperties += string.Format("--svn-show-copies-as-adds={0} ", reviewProperties.CopiesAsAdds == true ? "y":"n");

            // Build up the review groups
            if (reviewProperties.Groups.Count != 0)
            {
                string reviewGroups = reviewProperties.Groups[0];
                for (int i = 1; i < reviewProperties.Groups.Count; ++i)
                    reviewGroups += "," + reviewProperties.Groups[i];

                // Add the command
                commandProperties += string.Format("--target-groups \"{0}\" ", reviewGroups);
            }

            // Get the name of the branch we're on
            string branchUrl = GetSvnBranch(workingDirectory);
            if (string.IsNullOrWhiteSpace(branchUrl) == false)
                commandProperties += string.Format("--branch \"{0}\" ", branchUrl);

            // Update an existing review?
            if (string.IsNullOrWhiteSpace(reviewProperties.ReviewId) == false)
                commandProperties += string.Format("--review-request-id \"{0}\" ", reviewProperties.ReviewId);

            // Pass through the patch file we generated or used
            commandProperties += string.Format("--diff-filename \"{0}\" ", reviewProperties.Contents.Patch);

            // Build up the command
            string commandOptions = string.Format(@"post {0}", commandProperties);
            string rbtPath = GetRbtPath();

            // Run the process
            Utilities.Process.Output output = Utilities.Process.Start(workingDirectory, rbtPath, commandOptions);

            // Throw to return the error
            if (string.IsNullOrWhiteSpace(output.StdErr) == false)
                throw new ApplicationException(output.StdErr);

            // Break up the results
            string[] outputLines = output.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            // Pull out the link to the review (use the diff)
            string reviewUrl = string.Empty;
            if (outputLines.Length >= 3)
                reviewUrl = outputLines[2];

            // Review URL needs to reference the reviewboard server
            string reviewError = null;
            if (reviewUrl.ToLower().Contains(server.ToLower()) == false)
                reviewError = output.StdOut;

            // Lose the temp files if we have them
            CleanUpTemporaryFiles(descriptionFile, testingFile);

            // Return the results
            return new ReviewRequestResult(reviewUrl, reviewError, reviewProperties);
        }

        // Private properties
        private static string s_rbtPath = null;

        //
        // Since Process.Start needs a full path, find where rbt.cmd is installed
        //
        private static string GetRbtPath()
        {
            if (string.IsNullOrWhiteSpace(s_rbtPath) == false)
                return s_rbtPath;

            // Get the path
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, "where", "rbt");

            // Break up the paths given
            string[] pathsReturned = output.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach(string thisPath in pathsReturned)
            {
                if (thisPath.ToLower().Contains("rbt.cmd") == true)
                    s_rbtPath = thisPath;

            }

            // Can't find it
            if (string.IsNullOrWhiteSpace(s_rbtPath) == true)
                throw new System.ArgumentNullException("Unable to find the rbt.cmd path");

            // Done
            return s_rbtPath;
        }

        //
        // Returns the current branch of SVN
        //
        private static string GetSvnBranch(string workingDirectory)
        {
            // Generate the info
            Utilities.Process.Output infoOutput = Utilities.Process.Start(workingDirectory, "svn", "info");
            if (string.IsNullOrWhiteSpace(infoOutput.StdOut) == true)
                return null;

            // Find the URL
            string[] output = infoOutput.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string thisLine in output)
            {
                string trimmedLine = thisLine.Trim();
                if (trimmedLine.StartsWith("URL: ") == true)
                {
                    string url = trimmedLine.Replace("URL: ", "");
                    return url;
                }
            }

            // Got here so it's not here
            return null;
        }

        //
        // Returns a temp file containing the string
        //
        private static string GetFileWithContents(string contents, string replacementContent)
        {
            // Get the text we'll use
            string textToUse = string.IsNullOrWhiteSpace(contents) == true ? replacementContent : contents;
            if (string.IsNullOrWhiteSpace(textToUse) == true)
                return null;

            // Build up the temp file
            string result = Path.GetTempFileName();
            File.WriteAllText(result, textToUse);

            return result;
        }

        //
        // Deletes the temporary files we created for the review
        //
        private static void CleanUpTemporaryFiles(string descriptionFile, string testingFile)
        {
            Utilities.Storage.Keep(descriptionFile, "Review Description.txt", true);
            Utilities.Storage.Keep(testingFile, "Review Testing.txt", true);
        }
    }
}
