using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;
using RB_Tools.Shared.Utilities;
using RB_Tools.Shared.Logging;
using RB_Tools.Shared.Server;

namespace Create_Review
{
    class Reviewboard
    {
        // Returns the result of a review request
        public class ReviewRequestResult
        {
            public readonly string                      Url;
            public readonly string                      Error;

            public readonly Review.Review.Properties    Properties;

            // Constructor
            public ReviewRequestResult(string reviewUrl, string reviewError, Review.Review.Properties reviewRequest)
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
        // Returns a list of review groups from the given server
        //
        public static ReviewGroup[] GetReviewGroups(string workingDirectory, string server, string username, string password, Logging logger)
        {
            // Build up the command
            string commandOptions = string.Format(@"api-get --server {0} /groups/", server);
            string rbtPath = RB_Tools.Shared.Targets.Reviewboard.Path();

            // Update process
            logger.Log("Getting review groups");
            logger.Log("* Calling {0} {1}", rbtPath, commandOptions);

            // Run the process
            Process.Output output = Process.Start(workingDirectory, rbtPath, commandOptions);

            // Throw to return the error
            if (string.IsNullOrWhiteSpace(output.StdErr) == false)
            {
                logger.Log("Error encountered when getting review groups\n\n{0}", output.StdErr);
                throw new ApplicationException(output.StdErr);
            }

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
                logger.Log(" * Found group - {0} : {1}", searchResult.display_name, searchResult.name);
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
            logger.Log(" * Found {0} groups", groups.Length);
            return groups;
        }

        //
        // Raises a new review
        //
        public static ReviewRequestResult RequestReview(string workingDirectory, string server, string username, string password, Review.Review.Properties reviewProperties, Logging logger)
        {
            // We may not need to generate a review
            if (reviewProperties.ReviewLevel != RB_Tools.Shared.Review.Properties.Level.FullReview)
            {
                // No review needed so exit
                logger.Log("Ignoring review request as a full review was not requested");
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
            {
                string formattedJiraIds = BuildJiraLinks(reviewProperties.JiraId);
                commandProperties += string.Format("--bugs-closed \"{0}\" ", formattedJiraIds);
            }

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
            string branchUrl = Svn.GetBranch(workingDirectory);
            if (string.IsNullOrWhiteSpace(branchUrl) == false)
                commandProperties += string.Format("--branch \"{0}\" ", branchUrl);

            // Update an existing review?
            if (string.IsNullOrWhiteSpace(reviewProperties.ReviewId) == false)
                commandProperties += string.Format("--review-request-id \"{0}\" ", reviewProperties.ReviewId);

            // Pass through the patch file we generated or used
            commandProperties += string.Format("--diff-filename \"{0}\" ", reviewProperties.Contents.Patch);

            // Build up the command
            string commandOptions = string.Format(@"post {0}", commandProperties);
            string rbtPath = RB_Tools.Shared.Targets.Reviewboard.Path();

            logger.Log("Requesting review");
            logger.Log("* Calling {0} {1}", rbtPath, commandOptions);

            // Run the process
            Process.Output output = Process.Start(workingDirectory, rbtPath, commandOptions);

            // Throw to return the error
            if (string.IsNullOrWhiteSpace(output.StdErr) == false)
            {
                logger.Log("Error raised when requesting review - {0}", output.StdErr);
                throw new ApplicationException(output.StdErr);
            }

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
            CleanUpTemporaryFiles(descriptionFile, testingFile, logger);

            // Return the results
            return new ReviewRequestResult(reviewUrl, reviewError, reviewProperties);
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
        private static void CleanUpTemporaryFiles(string descriptionFile, string testingFile, Logging logger)
        {
            Utilities.Storage.Keep(descriptionFile, "Review Description.txt", true, logger);
            Utilities.Storage.Keep(testingFile, "Review Testing.txt", true, logger);
        }

        //
        // Builds up the Jira links
        //
        private static string BuildJiraLinks(string jiraIds)
        {
            string jiraLinks = string.Empty;

            // Build up the IDs we needs
            string[] jiras = jiraIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (jiras == null || jiras.Length == 0)
                jiras = new string[] { jiraIds };

            // Add the links
            string jiraServer = Names.Url[(int)Names.Type.Jira];
            for (int i = 0; i < jiras.Length; ++i)
            {
                string thisJiraLink = string.Format("{0}/browse/{1} ", jiraServer, jiras[i].Trim());
                jiraLinks += thisJiraLink;
            }
            
            // Send the tag back
            return jiraLinks;
        }
    }
}
