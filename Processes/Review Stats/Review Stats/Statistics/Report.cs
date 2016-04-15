using RB_Tools.Shared.Server;
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
            outputContent = UpdateReviewStatistics(outputContent, reviewStats);
            outputContent = UpdateAverageResults(outputContent, reviewStats);
            outputContent = UpdateReviewLists(outputContent, reviewStats);
            outputContent = UpdateCopyrightSection(outputContent);

            // Display the content
            DisplayReport(outputContent, revisions.Url);

            return true;
        }

        // Private properties
        private const string NumberFormat = @"N0";
        private const string PercentageFormat = @"n0";
        private const string RatioFormat = @"n2";
        private const string DateFormat = @"r";

        private const string Copyright = @"";

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
        // Updates the review stats of the report
        //
        private static string UpdateReviewStatistics(string outputContent, ReviewState.ReviewStatistics[] reviewStats)
        {
            // Update our values
            outputContent = outputContent.Replace(@"___REVIEW_TOTAL___", reviewStats.Length.ToString(NumberFormat));

            // Closed reviews
            var closedReviews = CountReviews(reviewStats, ReviewState.State.Closed);
            var closedShippedCount = GetPercentageBreakdown(closedReviews.First, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_CLOSED_SHIPPED___", closedShippedCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_CLOSED_SHIPPED_PERCENTAGE___", closedShippedCount.Second);

            var closedNotShippedCount = GetPercentageBreakdown(closedReviews.Second, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_CLOSED_NOT_SHIPPED___", closedNotShippedCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_CLOSED_NOT_SHIPPED_PERCENTAGE___", closedNotShippedCount.Second);

            // Open reviews
            var openedReviews = CountReviews(reviewStats, ReviewState.State.Open);
            var openedShippedCount = GetPercentageBreakdown(openedReviews.First, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_OPEN_SHIPPED___", openedShippedCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_OPEN_SHIPPED_PERCENTAGE___", openedShippedCount.Second);

            var openedNotShippedCount = GetPercentageBreakdown(openedReviews.Second, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_OPEN_NOT_SHIPPED___", openedNotShippedCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_OPEN_NOT_SHIPPED_PERCENTAGE___", openedNotShippedCount.Second);
            
            // Pending reviews
            var pendingReviews = CountReviews(reviewStats, ReviewState.State.Pending);
            var pendingCount = GetPercentageBreakdown(pendingReviews.First + pendingReviews.Second, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_PENDING___", pendingCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_PENDING_PERCENTAGE___", pendingCount.Second);

            // Discarded reviews
            var discardedReviews = CountReviews(reviewStats, ReviewState.State.Discarded);
            var discardedCount = GetPercentageBreakdown(discardedReviews.First + discardedReviews.Second, reviewStats.Length);
            outputContent = outputContent.Replace(@"___REVIEW_DISCARDED___", discardedCount.First);
            outputContent = outputContent.Replace(@"___REVIEW_DISCARDED_PERCENTAGE___", discardedCount.Second);

            // Return our updated report
            return outputContent;
        }

        //
        // Updates the averages section
        //
        private static string UpdateAverageResults(string outputContent, ReviewState.ReviewStatistics[] reviewStats)
        {
            // Get our counts
            int numberOfReviews = 0, numberOfReplies = 0, numberOfShipIts = 0;
            foreach (ReviewState.ReviewStatistics thisReview in reviewStats)
            {
                numberOfShipIts += thisReview.ShipIts;
                numberOfReplies += thisReview.Reviews - thisReview.Replies;
                numberOfReviews += thisReview.Reviews;
            }

            // Update our values
            float reviewsPerReview = numberOfReviews / (float)reviewStats.Length;
            outputContent = outputContent.Replace(@"___REVIEWS_PER_REVIEW___", reviewsPerReview.ToString(RatioFormat));

            float repliesPerReview = numberOfReplies / (float)reviewStats.Length;
            outputContent = outputContent.Replace(@"___REPLIES_PER_REVIEW___", repliesPerReview.ToString(RatioFormat));

            float shipItsPerReview = numberOfShipIts / (float)reviewStats.Length;
            outputContent = outputContent.Replace(@"___SHIP_ITS_PER_REVIEW___", shipItsPerReview.ToString(RatioFormat));

            // Return our updated report
            return outputContent;
        }

        //
        // Displays the list of reviews in various sections
        //
        private static string UpdateReviewLists(string outputContent, ReviewState.ReviewStatistics[] reviewStats)
        {
            // Pull our our reviews
            var openReviews = WhereReviews(reviewStats, ReviewState.State.Open);
            var closedReviews = WhereReviews(reviewStats, ReviewState.State.Closed);
            var pendingReviews = WhereReviews(reviewStats, ReviewState.State.Pending);
            var discardedReviews = WhereReviews(reviewStats, ReviewState.State.Discarded);
            var notFoundReviews = WhereReviews(reviewStats, ReviewState.State.NotFound);

            outputContent = CreateReviewTable(outputContent, openReviews.First, openReviews.Second, @"Open Reviews", @"___OPEN_REVIEWS_TABLE_ENTRY___", true);
            outputContent = CreateReviewTable(outputContent, pendingReviews.First, pendingReviews.Second, @"Pending Reviews", @"___PENDING_REVIEWS_TABLE_ENTRY___", true);
            outputContent = CreateReviewTable(outputContent, discardedReviews.First, discardedReviews.Second, @"Discarded Reviews", @"___DISCARDED_REVIEWS_TABLE_ENTRY___", true);
            outputContent = CreateReviewTable(outputContent, closedReviews.First, closedReviews.Second, @"Closed Reviews", @"___CLOSED_REVIEWS_TABLE_ENTRY___", true);
            outputContent = CreateReviewTable(outputContent, notFoundReviews.First, notFoundReviews.Second, @"Reviews Not Found", @"___INVALID_REVIEWS_TABLE_ENTRY___", false);

            // Return our output
            return outputContent;
        }

        //
        // Updates the copyright section
        //
        private static string UpdateCopyrightSection(string outputContent)
        {
            // Update it and return
            outputContent = outputContent.Replace(@"___COPYRIGHT_STATEMENT___", Copyright);
            return outputContent;
        }


        //
        // Creates the review table displays
        //
        private static string CreateReviewTable(string outputContent, ReviewState.ReviewStatistics[] shippedReviews, ReviewState.ReviewStatistics[] notShippedReviews, string title, string templateId, bool includeMetrics)
        {
            // If we have none, just clear the template
            if (shippedReviews.Length == 0 && notShippedReviews.Length == 0)
                return outputContent.Replace(templateId, @"");

            // Build up our components
            string titleSection = string.Format(@"<br><heading_3>{0}</heading_3><br><br>", title);
            string shippedTable = CreateInidividualTable(shippedReviews, includeMetrics);
            string notShippedTable = CreateInidividualTable(notShippedReviews, includeMetrics);

            // Build up the final result and inject
            string finalTable = string.Format("{0}\n{1}\n{2}", titleSection, shippedTable, notShippedTable);
            outputContent = outputContent.Replace(templateId, finalTable);

            // Return our output
            return outputContent;
        }

        //
        // Creates a single table of reviews
        //
        private static string CreateInidividualTable(ReviewState.ReviewStatistics[] reviewList, bool includeMetrics)
        {
            // We need something in the list
            if (reviewList.Length == 0)
                return string.Empty;

            // Build up the structure
            StringBuilder tableHeader = new StringBuilder();
            tableHeader.Append(@"<table>
	                                <tr>
		                                <td width=""100px""><heading_table>Revision</heading_table></td>
                                        <td width=""100px""><heading_table>Author</heading_table></td>
		                                <td width=""200px""><heading_table>Review</heading_table></td>");
            if (includeMetrics == true)
            {
                tableHeader.Append(@"<td width=""75px""><heading_table>Ship It's</heading_table></td>
		                                <td width=""200px""><heading_table>Additional Comments</heading_table></td>");
            }
            tableHeader.Append(@"</tr>");

            StringBuilder rowFormat = new StringBuilder();
            rowFormat.Append(@"<tr>
		                            <td>{0}</td>
		                            <td>{1}</td>
                                    <td>{2}</td>");
            if (includeMetrics == true)
            {
                rowFormat.Append(@"<td>{3}</td>
		                            <td>{4}</td>");
            }
            rowFormat.Append(@"</tr>");
            string tableFooter = @"</table><br><br>";
            string reviewLinkFormat = @"<a href=""{0}"" target=""_blank"">Review #{1}</a>";

            // Build up the table
            StringBuilder reviewRows = new StringBuilder();
            foreach (ReviewState.ReviewStatistics thisReview in reviewList)
            {
                // Build up the row data
                string reviewId = ExtractReviewId(thisReview.Commit.Review);
                string reviewCode = string.Format(reviewLinkFormat, thisReview.Commit.Review, reviewId);
                string thisRow = string.Format(rowFormat.ToString(),  thisReview.Commit.Log.Revision.ToString(NumberFormat),
                                                                      TrimAuthor(thisReview.Commit.Log.Author),
                                                                      reviewCode,
                                                                      thisReview.ShipIts.ToString(NumberFormat),
                                                                      thisReview.Replies.ToString(NumberFormat));
                // Add this row
                reviewRows.AppendLine(thisRow);
            }

            // Build up the table
            string finalTable = string.Format("{0}\n{1}\n{2}", tableHeader, reviewRows.ToString(), tableFooter);
            return finalTable;
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

        //
        // Returns the number of reviews with this state
        //
        private static Pair<int, int> CountReviews(ReviewState.ReviewStatistics[] reviewStats, ReviewState.State requestedState)
        {
            var reviews = WhereReviews(reviewStats, requestedState);
            return new Pair<int, int>(reviews.First.Length, reviews.Second.Length);
        }

        //
        // Returns the reviews with the given state
        //
        private static Pair<ReviewState.ReviewStatistics[], ReviewState.ReviewStatistics[]> WhereReviews(ReviewState.ReviewStatistics[] reviewStats, ReviewState.State requestedState)
        {
            var shippedReviews = reviewStats.Where(thisReview => thisReview.State == requestedState && thisReview.ShipIts > 0);
            var notshippedReviews = reviewStats.Where(thisReview => thisReview.State == requestedState && thisReview.ShipIts == 0);

            return new Pair<ReviewState.ReviewStatistics[], ReviewState.ReviewStatistics[]>(shippedReviews.ToArray(), notshippedReviews.ToArray());
        }

        //
        // Pulls out the review ID
        //
        private static string ExtractReviewId(string review)
        {
            string cleanUrl = Paths.Clean(review);
            string urlPath = Names.Url[(int)Names.Type.Reviewboard] + @"/r/";

            // Strip out the path
            string reviewId = cleanUrl.Replace(urlPath, "");
            return reviewId;
        }

        //
        // Trims the username if needed
        //
        private static string TrimAuthor(string author)
        {
            int autherLengthMax = 13;
            if (author.Length <= autherLengthMax)
                return author;

            // Chop off the end
            string trimmedAuthor = author.Substring(0, autherLengthMax);
            return trimmedAuthor + @"...";
        }
    }
}
