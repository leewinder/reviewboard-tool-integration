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
using RB_Tools.Shared.Logging;

namespace Review_Stats.Statistics
{
    class Generator
    {
        // Public delegates
        public delegate void GenerationFinished();

        //
        // Start the process of generating the stats
        //
        public static void Start(Form owner, string fileList, Logging logger, bool injectPaths, GenerationFinished generationFinished)
        {
            // Track our properties
            s_logger = logger;

            // Kick off the background threads
            BackgroundWorker updateThread = new BackgroundWorker();

            // Does the work of the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                logger.Log(@"Starting stats generation");
                try
                {
                    // Check if we're authenticated
                    Simple reviewBoardCredentials = CheckRBServerAuthentication(owner);
                    if (reviewBoardCredentials == null)
                        return;
                    Simple jiraCredentials = CheckJiraServerAuthentication(owner);
                    if (reviewBoardCredentials == null)
                        return;

                    // Get the list of paths to review
                    string[] pathsToReview = ParseFileList(fileList, injectPaths);
                    if (pathsToReview == null)
                        return;

                    // Get the revision list for each path
                    RevisionList.Revisions[] revisionLists = RequestRevisionLists(pathsToReview);
                    if (revisionLists == null)
                        return;

                    // Spin through each revision list and do the work for each path selected
                    foreach (RevisionList.Revisions thisRevisionList in revisionLists)
                    {
                        // Identify which path we'll go through 
                        s_logger.Log(@"Generating stats for\n{0}", thisRevisionList.Path);

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

                        // Now generate information about the reviews
                        ReviewState.ReviewStatistics[] reviewStats = GetReviewStats(commitStats.Reviews, thisRevisionList.Path, reviewBoardCredentials);
                        if (reviewStats == null)
                            return;

                        // Get the Jira information from the commits
                        JiraState.JiraStatistics jiraStats = GetJiraStats(revisionLogs, jiraCredentials);
                        if (jiraStats == null)
                            return;

                        // Create the review
                        bool reportGenerated = CreateReviewReport(thisRevisionList, revisionLogs, commitStats, reviewStats, jiraStats, stopWatch);
                        if (reportGenerated == false)
                            return;
                    }
                }
                catch (Exception e)
                {
                    s_logger.Log("Exception raised when generating review stats\n\n{0}\n", e.Message);
                    s_errorMessage = string.Format("Unable to generate review statistics for the given path\n\n{0}", e.Message);
                }
            };

            // Called when it's all been generated
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                s_logger.Log("Review generation complete");

                // Inform the user something went wrong?
                if (string.IsNullOrEmpty(s_errorMessage) == false)
                {
                    s_logger.Log(@"* Error encountered when running\n{0}\n", s_errorMessage);
                    MessageBox.Show(s_errorMessage, @"Stats Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Done
                generationFinished();
            };

            // Off it goes
            updateThread.RunWorkerAsync();
        }

        // Private properties
        private static string   s_errorMessage = null;
        private static Logging  s_logger = null;

        //
        // Parses and checks the file list
        //
        private static string[] ParseFileList(string fileList, bool injectPaths)
        {
            s_logger.Log(@"Staring command line parsing");

            // Parse the file
            Utilities.CommandRequests.Result parseResult = Utilities.CommandRequests.ParseCommands(fileList, injectPaths, s_logger);
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
        // Returns if we're authenticated against our RB server
        //
        private static Simple CheckRBServerAuthentication(Form owner)
        {
            // Get our server
            string reviewboardServer = Names.Url[(int)Names.Type.Reviewboard];

            // Check our credentials
            s_logger.Log(@"Checking Reviewboard credentials against {0}", reviewboardServer);
            if (Credentials.Available(reviewboardServer) == false)
            {
                // Kick off the request
                s_logger.Log(@"Reviewboard credentials not found for {0}", reviewboardServer);
                owner.Invoke((MethodInvoker)delegate
                {
                    DialogResult dialogResult = MessageBox.Show(owner, "You must be authenticated with the Reviewboard server before generating review statistics.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(s_logger);
                });

                // Check if we're still unauthenticated
                s_logger.Log(@"Reviewboard authentication completed");
                if (Credentials.Available(reviewboardServer) == false)
                {
                    s_logger.Log(@"Reviewboard authentication still not present");
                    return null;
                }
            }

            // We're good
            s_logger.Log(@"Creating Reviewboard credentials object for {0}", reviewboardServer);
            return Credentials.Create(reviewboardServer, s_logger) as Simple;
        }

        //
        // Checks we're authenticated against the Jira server
        //
        private static Simple CheckJiraServerAuthentication(Form owner)
        {
            // Get our server
            string jiraServer = Names.Url[(int)Names.Type.Jira];

            // Check our credentials
            s_logger.Log(@"Checking Jira credentials against {0}", jiraServer);
            if (Credentials.Available(jiraServer) == false)
            {
                // Kick off the request
                s_logger.Log(@"Jira credentials not found for {0}", jiraServer);
                owner.Invoke((MethodInvoker)delegate
                {
                    DialogResult dialogResult = MessageBox.Show(owner, "You must be authenticated with the Jira server before generating review statistics.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        RB_Tools.Shared.Authentication.Targets.Jira.Authenticate(s_logger);
                });

                // Check if we're still unauthenticated
                s_logger.Log(@"Jira authentication completed");
                if (Credentials.Available(jiraServer) == false)
                {
                    s_logger.Log(@"Jira authentication still not present");
                    return null;
                }
            }

            // We're good
            s_logger.Log(@"Creating Jira credentials object for {0}", jiraServer);
            return Credentials.Create(jiraServer, s_logger) as Simple;
        }
        

        //
        // Gets the list of revisions to review
        //
        private static RevisionList.Revisions[] RequestRevisionLists(string[] pathsToReview)
        {
            s_logger.Log(@"Identifying revisions list for all paths");
            RevisionList.Revisions[] results = RevisionList.Request(pathsToReview, s_logger);
            if (results == null)
                s_errorMessage = @"No valid revisions were selected to review";

            // Return our results
            s_logger.Log(@"Revisions identified");
            return results;
        }

        //
        // Pulls out the logs for given revisions
        //
        private static SvnLogs.Log[] GetLogsFromRevisions(RevisionList.Revisions revisionsToLog)
        {
            // Starting to review logs
            s_logger.Log(@"Getting logs for\n{0}", revisionsToLog.Revision);
            Display.Start(Display.State.ExtractingLogs, revisionsToLog.Revision.Length);

            // Get our results
            SvnLogs.Log[] results = SvnLogs.GetRevisionLogs(revisionsToLog.Path, revisionsToLog.Revision, s_logger, (currentCount) =>
            {
                Display.Update(currentCount, revisionsToLog.Revision.Length);
            });

            // Did we fail?
            if (results == null)
                s_errorMessage = @"Unable to get the logs for the revisions selected in " + revisionsToLog.Path;

            // Return our results
            s_logger.Log(@"Logs recieved");
            return results;
        }

        //
        // Gets the state of various commits
        //
        private static ReviewState.GetCommitStatsResult GetCommitStats(SvnLogs.Log[] revisionLogs)
        {
            // Starting to pasring
            s_logger.Log(@"Starting to generate the commit statistics");
            Display.Start(Display.State.ParsingLogs, revisionLogs.Length);

            // Get the stats about these commits
            ReviewState.GetCommitStatsResult results = ReviewState.GetCommitStatistics(revisionLogs, s_logger, (currentCount) =>
            {
                Display.Update(currentCount, revisionLogs.Length);
            });

            // Did we fail?
            if (results == null)
                s_errorMessage = @"Unable to generate the commit stats";

            // Return our results
            s_logger.Log(@"Commit stats generated");
            return results;
        }

        //
        // We should have the same number of reviews as we do commits
        //
        private static bool CheckReviewInformation(ReviewState.GetCommitStatsResult commitStats)
        {
            // Check we have the right number of issues
            int commitCount = commitStats.CommitCount[(int)RB_Tools.Shared.Review.Properties.Level.FullReview];

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
            s_logger.Log("Starting to extract the review stats");
            Display.Start(Display.State.QueryingReviewboard, reviews.Length);

            // Try to query the server for our review state
            try
            {
                ReviewState.ReviewStatistics[] results = ReviewState.GetReviewStatistics(reviews, workingDirectory, credentials, s_logger, (currentCount) =>
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
                s_logger.Log("Review stats generated");
                return results;
            }
            catch (ReviewboardApiException e)
            {
                // API error
                s_errorMessage = "Unable to generate the review stats against the RB server\n\n" + e.Message;
                return null;
            }
            catch (Exception generalException)
            {
                String exceptionMessage = (generalException.InnerException == null ? generalException.Message : generalException.InnerException.Message);
                s_errorMessage = "Unable to generate the review stats against the RB server\n\n" + exceptionMessage;
                return null;
            }
        }

        //
        // Returns stats about the jira use in this repository
        //
        private static JiraState.JiraStatistics GetJiraStats(SvnLogs.Log[] revisionLogs, Simple credentials)
        {
            // Starting to review logs
            s_logger.Log("Starting to extract the Jira stats");
            Display.Start(Display.State.QueryingReviewboard, revisionLogs.Length);

            // Try to query the server for our review state
            try
            {
                JiraState.JiraStatistics results = JiraState.GetJiraStatistics(revisionLogs, credentials, s_logger, (currentCount) =>
                {
                    Display.Update(currentCount, revisionLogs.Length);
                });

                // Did we fail
                if (results == null)
                {
                    s_errorMessage = @"Unable to generate the Jira stats against the Jira server";
                    return null;
                }

                // Return what we have
                s_logger.Log("Jira stats generated");
                return results;
            }
            catch (Exception generalException)
            {
                String exceptionMessage = (generalException.InnerException == null ? generalException.Message : generalException.InnerException.Message);
                s_errorMessage = "Unable to generate the Jira stats against the Jira server\n\n" + exceptionMessage;
                return null;
            }
        }

        //
        // Generates the report
        //
        private static bool CreateReviewReport(RevisionList.Revisions revisions, SvnLogs.Log[] revisionLogs, ReviewState.GetCommitStatsResult commitStats, ReviewState.ReviewStatistics[] reviewStats, JiraState.JiraStatistics jiraStats, Stopwatch reviewTimer)
        {
            // We're now generating
            s_logger.Log("Starting to generate review report");
            Display.Start(Display.State.CreatingResults);
            
            // Try and generate the report
            bool generated = Report.Generate(revisions, revisionLogs, commitStats, reviewStats, jiraStats, s_logger, reviewTimer.Elapsed);
            if (generated == false)
                s_errorMessage = @"Unable to generate the Review Report";

            // Return our results
            return generated;
        }

    }
}
