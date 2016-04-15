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
using System.Diagnostics;

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
                    // We need to time this review
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    // Get the logs for the given set of revisions
                    SvnLogs.Log[] revisionLogs = GetLogsFromRevisions(thisRevisionList);
                    if (revisionLogs == null)
                        return;

                    ReviewState.GetCommitStatsResult commitStats = GetCommitStats(revisionLogs);
                    if (commitStats == null)
                        return;

                    // Verify would could get the review information
                    bool reviewInformationValid = CheckReviewInformation(commitStats);
                    if (reviewInformationValid == false)
                        return;

                    // Now generate unformation about the reviews
                    ReviewState.ReviewStatistics[] reviewStats = GetReviewStats(commitStats.Reviews, thisRevisionList.Path, credentials);
                    if (reviewStats == null)
                        return;

                    // Create the review
                    bool reportGenerated = CreateReviewReport(thisRevisionList, revisionLogs, commitStats, reviewStats, stopWatch);
                    if (reportGenerated == false)
                        return;
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
        // We should have the same number of reviews as we do commits
        //
        private static bool CheckReviewInformation(ReviewState.GetCommitStatsResult commitStats)
        {
            int commitCount = 0;
            foreach (int thisCommitCount in commitStats.CommitCount)
                commitCount += thisCommitCount;

            // If they are not the same, we have an issue
            if (commitCount != commitStats.Reviews.Length)
            {
                s_errorMessage = @"Unable to find the reviews listed in the commits, which means the Reviewboard server does not match the the one authenticated against";
                return false;
            }

            // We're OK
            return true;
        }

        //
        // Returns stats about the reviews in this repository
        //
        private static ReviewState.ReviewStatistics[] GetReviewStats(ReviewState.CommitReview[] reviews, string workingDirectory, Simple credentials)
        {
            // Starting to review logs
            Display.Start(Display.State.QueryingReviewboard, reviews.Length);

            // Try to query the server for our review state
            try
            {
                ReviewState.ReviewStatistics[] results = ReviewState.GetReviewStatistics(reviews, workingDirectory, credentials, (currentCount) =>
                {
                    Display.Update(currentCount, reviews.Length);
                });

                // Did we fail
                if (results == null)
                {
                    s_errorMessage = @"Unable to generate the review stats against the RB server";
                    return null;
                }

                // Return what we have
                return results;
            }
            catch (ReviewboardApiException e)
            {
                // API error
                s_errorMessage = "Unable to generate the review stats against the RB server\n\n" + e.Message;
                return null;
            }
        }

        //
        // Generates the report
        //
        private static bool CreateReviewReport(RevisionList.Revisions revisions, SvnLogs.Log[] revisionLogs, ReviewState.GetCommitStatsResult commitStats, ReviewState.ReviewStatistics[] reviewStats, Stopwatch reviewTimer)
        {
            // We're now generating
            Display.Start(Display.State.CreatingResults);
            
            // Try and generate the report
            bool generated = Report.Generate(revisions, revisionLogs, commitStats, reviewStats, reviewTimer.Elapsed);
            if (generated == false)
                s_errorMessage = @"Unable to generate the Review Report";

            // Return our results
            return generated;
        }

    }
}
