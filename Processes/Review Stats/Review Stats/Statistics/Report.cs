using RB_Tools.Shared.Utilities;
using Review_Stats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Review_Stats.Statistics
{
    //
    // Generates the report to display to the user
    //
    class Report
    {
        //
        // Generates the final report
        //
        public static bool Generate(RevisionList.Revisions revisions, SvnLogs.Log[] revisionLogs, ReviewState.GetCommitStatsResult commitStats, ReviewState.ReviewStatistics[] reviewStats, TimeSpan reviewTime)
        {
            // Start with the clean template
            string outputContent = Properties.Resources.ReportTemplate;

            // Spin through and update various sections
            outputContent = UpdateOverview(outputContent, revisionLogs, revisions.Url, reviewTime);

            // Display the content
            DisplayReport(outputContent, revisions.Url);

            return true;
        }

        // Private properties
        private const string NumberFormatter = @"N0";
        private const string DateFormatter = @"r";

        //
        // Updates the overview properties for the review
        //
        private static string UpdateOverview(string outputContent, SvnLogs.Log[] revisionLogs, string url, TimeSpan reviewTime)
        {
            // Get our log range
            SvnLogs.Log startLog = revisionLogs[revisionLogs.Length - 1];
            SvnLogs.Log endLog = revisionLogs[0];

            // Get the repository name
            string repositoryName = GetRepositoryName(url);

            outputContent = outputContent.Replace(@"___REPOSITORY_NAME___", repositoryName);
            outputContent = outputContent.Replace(@"___REPOSITORY_URL___", url);

            outputContent = outputContent.Replace(@"___REVISION_RANGE_MIN___", startLog.Revision.ToString(NumberFormatter));
            outputContent = outputContent.Replace(@"___REVISION_RANGE_MAX___", endLog.Revision.ToString(NumberFormatter));

            outputContent = outputContent.Replace(@"___RANGE_DATE_START___", startLog.Date.ToUniversalTime().ToString(DateFormatter));
            outputContent = outputContent.Replace(@"___RANGE_DATE_END___", endLog.Date.ToUniversalTime().ToString(DateFormatter));

            outputContent = outputContent.Replace(@"___REVIEW_DATE___", DateTime.Now.ToUniversalTime().ToString(DateFormatter));

            string reviewLength = string.Format(@"{0:D2}h:{1:D2}m:{2:D2}s",
                        reviewTime.Hours,
                        reviewTime.Minutes,
                        reviewTime.Seconds);
            outputContent = outputContent.Replace(@"___REVIEW_DURATION___", reviewLength);

            return outputContent;
        }

        //
        // Gets the name of the repository
        //
        private static string GetRepositoryName(string repository)
        {
            return Path.GetFileName(repository);
        }

        //
        // Displays the report to the user
        //
        private static void DisplayReport(string outputContent, string url)
        {
            // Get our folder
            string reportsFolder = Paths.GetDocumentsFolder() + @"\Reports";
            Directory.CreateDirectory(reportsFolder);

            // Output file
            string currentDate = DateTime.Now.ToUniversalTime().ToString(DateFormatter).Replace(",", "").Replace(":", "-");
            string filePath = string.Format(@"{0}\{1} {2}.html", reportsFolder, GetRepositoryName(url), currentDate);

            // Write our file and show it
            File.WriteAllText(filePath, outputContent);
            System.Diagnostics.Process.Start(filePath);
        }
    }
}
