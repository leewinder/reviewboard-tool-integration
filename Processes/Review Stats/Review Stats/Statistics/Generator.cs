using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Review_Stats.Utilities;
using RB_Tools.Shared.Server;
using RB_Tools.Shared.Authentication.Credentials;
using Review_Stats.Exceptions;

namespace Review_Stats.Statistics
{
    class Generator
    {
        // Public delegates
        public delegate void GenerationFinished();

        //
        // Start the process of generating the stats
        //
        public static void Start(Form owner, string fileList, string debugOptions, GenerationFinished generationFinished)
        {
            // Kick off the background threads
            BackgroundWorker updateThread = new BackgroundWorker();

            // Does the work of the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Check if we're authenticated
                Simple credentials = CheckServerAuthentication(owner);
                if (credentials == null)
                    return;

                // Get the list of paths to review
                string[] pathsToReview = ParseFileList(fileList, debugOptions);
                if (pathsToReview == null)
                    return;

                // Get the revision list for each path
                RevisionList.Revisions[] revisionLists = RequestRevisionLists(pathsToReview);
                if (revisionLists == null)
                    return;

                // Spin through each revision list and do the work for each path selected
                foreach (RevisionList.Revisions thisRevisionList in revisionLists)
                {
                    // Get the logs for the given set of revisions
                    SvnLogs.Log[] revisionLogs = GetLogsFromRevisions(thisRevisionList);
                    if (revisionLogs == null)
                        return;

                    ReviewState.GetCommitStatsResult commitStats = GetCommitStats(revisionLogs);
                    if (commitStats == null)
                        return;

                    // Now generate unformation about the reviews
                    ReviewState.GetReviewStatisticsResult reviewStats = GetReviewStats(commitStats.Reviews, thisRevisionList.Path, credentials);
                    if (reviewStats == null)
                        return;

                    // Create the review
                    Display.Start(Display.State.CreatingResults);
                }
            };

            // Called when it's all been generated
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Inform the user something went wrong?
                if (string.IsNullOrEmpty(s_errorMessage) == false)
                    MessageBox.Show(s_errorMessage, @"Stats Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Done
                generationFinished();
            };

            // Off it goes
            updateThread.RunWorkerAsync();
        }

        // Private properties
        private static string s_errorMessage = null;

        //
        // Parses and checks the file list
        //
        private static string[] ParseFileList(string fileList, string debugOptions)
        {
            // Parse the file
            Utilities.CommandRequests.Result parseResult = Utilities.CommandRequests.ParseCommands(fileList, debugOptions);
            if (parseResult == null)
            {
                s_errorMessage = @"Unable to read the given file list data";
                return null;
            }

            // If we have no valid files, then bail
            if (parseResult.ValidPaths.Length == 0)
            {
                s_errorMessage = @"Unable to find any paths that were valid SVN repositories";
                return null;
            }

            // Let the user know which ones we're ignoring
            if (parseResult.InvalidPaths.Length != 0)
            {
                s_errorMessage = "The following paths will be ignored as they are not valid SVN repositories\n";
                foreach (string thisPath in parseResult.InvalidPaths)
                {
                    string shortPath = Paths.TruncateLongPath(thisPath);
                    s_errorMessage += string.Format(@"- {0}", shortPath);
                }

                // Nothing
                return null;
            }

            // Return our valid files
            return parseResult.ValidPaths;
        }

        //
        // Returns if we're authenticated against our servers
        //
        private static Simple CheckServerAuthentication(Form owner)
        {
            string reviewboardServer = Names.Url[(int)Names.Type.Reviewboard];
            if (Credentials.Available(reviewboardServer) == false)
            {
                // Kick off the request
                owner.Invoke((MethodInvoker)delegate
                {
                    DialogResult dialogResult = MessageBox.Show(owner, "You must be authenticated with the Reviewboard server before generating review statistics.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate();
                });

                // Check if we're still unauthenticated
                if (Credentials.Available(reviewboardServer) == false)
                    return null;
            }

            // We're good
            return Credentials.Create(reviewboardServer) as Simple;
        }

        //
        // Gets the list of revisions to review
        //
        private static RevisionList.Revisions[] RequestRevisionLists(string[] pathsToReview)
        {
            RevisionList.Revisions[] results = RevisionList.Request(pathsToReview);
            if (results == null)
                s_errorMessage = @"No valid revisions were selected to review";

            // Return our results
            return results;
        }

        //
        // Pulls out the logs for given revisions
        //
        private static SvnLogs.Log[] GetLogsFromRevisions(RevisionList.Revisions revisionsToLog)
        {
            // Starting to review logs
            Display.Start(Display.State.ExtractingLogs, revisionsToLog.Revision.Length);

            // Get our results
            SvnLogs.Log[] results = SvnLogs.GetRevisionLogs(revisionsToLog.Path, revisionsToLog.Revision, (currentCount) =>
            {
                Display.Update(currentCount, revisionsToLog.Revision.Length);
            });

            // Did we fail?
            if (results == null)
                s_errorMessage = @"Unable to get the logs for the revisions selected in " + revisionsToLog.Path;

            // Return our results
            return results;
        }

        //
        // Gets the state of various commits
        //
        private static ReviewState.GetCommitStatsResult GetCommitStats(SvnLogs.Log[] revisionLogs)
        {
            // Starting to pasring
            Display.Start(Display.State.ParsingLogs, revisionLogs.Length);

            // Get the stats about these commits
            ReviewState.GetCommitStatsResult results = ReviewState.GetCommitStatistics(revisionLogs, (currentCount) =>
            {
                Display.Update(currentCount, revisionLogs.Length);
            });

            // Did we fail?
            if (results == null)
                s_errorMessage = @"Unable to generate the commit stats";

            // Return our results
            return results;
        }

        //
        // Returns stats about the reviews in this repository
        //
        private static ReviewState.GetReviewStatisticsResult GetReviewStats(ReviewState.CommitReview[] reviews, string workingDirectory, Simple credentials)
        {
            // Starting to review logs
            Display.Start(Display.State.QueryingReviewboard, reviews.Length);

            // Try to query the server for our review state
            ReviewState.GetReviewStatisticsResult results = null;
            try
            {
                results = ReviewState.GetReviewStatistics(reviews, workingDirectory, credentials, (currentCount) =>
                {
                    Display.Update(currentCount, reviews.Length);
                });

                // Did we fail
                if (results == null)
                {
                    s_errorMessage = @"Unable to generate the review stats against the RB server";
                    return null;
                }
            }
            catch (ReviewboardApiException e)
            {
                // API error
                s_errorMessage = "Unable to generate the review stats against the RB server\n\n" + e.Message;
                return null;
            }

            // Return our results
            return results;
        }

    }
}
