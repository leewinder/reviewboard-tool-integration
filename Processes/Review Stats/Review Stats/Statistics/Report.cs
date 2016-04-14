using RB_Tools.Shared.Structures;
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
    // Doesn't do anything smart, just simple text replacment of the template
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
            outputContent = UpdateCommitStatistics(outputContent, commitStats);

            // Display the content
            DisplayReport(outputContent, revisions.Url);

            return true;
        }

        // Private properties
        private const string NumberFormat = @"N0";
        private const string PercentageFormat = @"n0";
        private const string DateFormat = @"r";

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

            outputContent = outputContent.Replace(@"___REVISION_RANGE_MIN___", startLog.Revision.ToString(NumberFormat));
            outputContent = outputContent.Replace(@"___REVISION_RANGE_MAX___", endLog.Revision.ToString(NumberFormat));

            outputContent = outputContent.Replace(@"___RANGE_DATE_START___", startLog.Date.ToUniversalTime().ToString(DateFormat));
            outputContent = outputContent.Replace(@"___RANGE_DATE_END___", endLog.Date.ToUniversalTime().ToString(DateFormat));

            outputContent = outputContent.Replace(@"___REVIEW_DATE___", DateTime.Now.ToUniversalTime().ToString(DateFormat));

            string reviewLength = string.Format(@"{0:D2}h:{1:D2}m:{2:D2}s",
                        reviewTime.Hours,
                        reviewTime.Minutes,
                        reviewTime.Seconds);
            outputContent = outputContent.Replace(@"___REVIEW_DURATION___", reviewLength);

            // Return our new report
            return outputContent;
        }

        //
        // Updates the commit stats for the report
        //
        private static string UpdateCommitStatistics(string outputContent, ReviewState.GetCommitStatsResult commitStats)
        {
            // Get the total commit count
            int totalCommits = commitStats.UnknownCommits;
            foreach (int commitNumber in commitStats.CommitCount)
                totalCommits += commitNumber;

            // Update our values
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_TOTAL___", totalCommits.ToString(NumberFormat));

            var reviewedCount = GetPercentageBreakdown(commitStats.CommitCount[(int)RB_Tools.Shared.Review.Properties.Level.FullReview], totalCommits);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_REVIEWED___", reviewedCount.First);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_REVIEWED_PERCENTAGE___", reviewedCount.Second);
            
            var laterCount = GetPercentageBreakdown(commitStats.CommitCount[(int)RB_Tools.Shared.Review.Properties.Level.PendingReview], totalCommits);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_LATER___", laterCount.First);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_LATER_PERCENTAGE___", laterCount.Second);

            var noCount = GetPercentageBreakdown(commitStats.CommitCount[(int)RB_Tools.Shared.Review.Properties.Level.NoReview], totalCommits);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_NO_REVIEW___", noCount.First);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_NO_REVIEW_PERCENTAGE___", noCount.Second);

            var assetCount = GetPercentageBreakdown(commitStats.CommitCount[(int)RB_Tools.Shared.Review.Properties.Level.AssetCommit], totalCommits);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_ASSET_ONLY___", assetCount.First);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_ASSET_ONLY_PERCENTAGE___", assetCount.Second);

            var unknownCount = GetPercentageBreakdown(commitStats.UnknownCommits, totalCommits);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_NONE___", unknownCount.First);
            outputContent = outputContent.Replace(@"___COMMIT_COUNT_NONE_PERCENTAGE___", unknownCount.Second);

            // Return our new report
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
            string currentDate = DateTime.Now.ToUniversalTime().ToString(DateFormat).Replace(",", "").Replace(":", "-");
            string filePath = string.Format(@"{0}\{1} {2}.html", reportsFolder, GetRepositoryName(url), currentDate);

            // Write our file and show it
            File.WriteAllText(filePath, outputContent);
            System.Diagnostics.Process.Start(filePath);
        }

        //
        // Returns an updated count
        //
        private static Pair<string, string> GetPercentageBreakdown(int actual, int max)
        {
            float percentage = (actual / (float)max) * 100.0f;
            return new Pair<string, string>(actual.ToString(NumberFormat), percentage.ToString(PercentageFormat));
        }
    }
}
