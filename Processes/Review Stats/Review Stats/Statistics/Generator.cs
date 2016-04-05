using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Review_Stats.Utilities;

namespace Review_Stats.Statistics
{
    class Generator
    {
        // Public delegates
        public delegate void GenerationFinished();

        //
        // Start the process of generating the stats
        //
        public static void Start(string fileList, string debugOptions, GenerationFinished generationFinished)
        {
            // Kick off the background threads
            BackgroundWorker updateThread = new BackgroundWorker();

            // Does the work of the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Get the list of paths to review
                string[] pathsToReview = ParseFileList(fileList, debugOptions);
                if (pathsToReview == null)
                    return;

                // Get the revision list for each path
                RevisionList.Result[] revisionLists = RequestRevisionLists(pathsToReview);
                if (revisionLists == null)
                    return;

                // Spin through each revision list and do the work for each path selected
                foreach (RevisionList.Result thisRevisionList in revisionLists)
                {
                    // Get the logs for the given set of revisions
                    SvnLogs.Result[] revisionLogs = GetLogsFromRevisions(thisRevisionList);
                    if (revisionLogs == null)
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
        // Gets the list of revisions to review
        //
        private static RevisionList.Result[] RequestRevisionLists(string[] pathsToReview)
        {
            RevisionList.Result[] results = RevisionList.Request(pathsToReview);
            if (results == null)
                s_errorMessage = @"No valid revisions were selected to review";

            // Return our results
            return results;
        }

        private static SvnLogs.Result[] GetLogsFromRevisions(RevisionList.Result revisionsToLog)
        {
            SvnLogs.Result[] results = SvnLogs.GetRevisionLogs(revisionsToLog.Path, revisionsToLog.Revisions);
            if (results == null)
                s_errorMessage = @"Unable to get the logs for the revisions selected in " + revisionsToLog.Path;

            // Return our results
            return results;
        }

    }
}
