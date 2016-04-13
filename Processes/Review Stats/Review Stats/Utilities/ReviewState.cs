using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Extensions;
using RB_Tools.Shared.Server;
using RB_Tools.Shared.Utilities;
using Review_Stats.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Review_Stats.Utilities
{
    // Provides information about reviews
    class ReviewState
    {
        // A commit with an associated review
        public class CommitReview
        {
            public int Revision { get; private set; }
            public string Review { get; private set; }

            // Constructor
            public CommitReview(int revision, string review)
            {
                Revision = revision;
                Review = review;
            }
        };

        // Results of commit review types
        public class GetCommitStatsResult
        {
            public int[]            CommitCount { get; private set; }
            public int              UnknownCommits { get; private set; }

            public CommitReview[]   Reviews { get; private set; }

            // Constructor
            public GetCommitStatsResult(int[] commitCount, int unknown, CommitReview[] reviews)
            {
                CommitCount = commitCount;
                UnknownCommits = unknown;

                Reviews = reviews;
            }
        };

        // Results of a review stats request
        public class GetReviewStatisticsResult
        {
            public CommitReview[] OpenReviewsWithShipIts { get; private set; }
            public CommitReview[] OpenReviewsNoShipIts { get; private set; }
            public CommitReview[] ClosedReviews { get; private set; }

            public int      CommentsGenerated { get; private set; }
            public int      ShipItsGenerated { get; private set; }

            // Constructor
            public GetReviewStatisticsResult(CommitReview[] shipItReviews, CommitReview[] noShipItReviews, CommitReview[] closedReviews, int commentCount, int shipItCount)
            {
                OpenReviewsWithShipIts = shipItReviews;
                OpenReviewsNoShipIts = noShipItReviews;
                ClosedReviews = closedReviews;

                CommentsGenerated = commentCount;
                ShipItsGenerated = shipItCount;
            }
        }

        //
        // Returns the statistics on the raised reviews
        //
        public static GetReviewStatisticsResult GetReviewStatistics(CommitReview[] reviewList, string workingDirectory, Simple credentials)
        {
            // We need to spin through every review and pull out the information about each one
            foreach (CommitReview thisReview in reviewList)
            {
                string reviewState = GetUrlReviewState(thisReview.Review, workingDirectory, credentials);
            }

            // Return our object
            return new GetReviewStatisticsResult(null, null, null, 0, 0);
        }

        //
        // Counts the type of reviews in a set of commits
        //
        public static GetCommitStatsResult GetCommitStats(SvnLogs.Log[] svnLogs)
        {
            // We need to track the types of reviews
            int unknownReviews = 0;
            int[] commitCounts = new int[Enum.GetNames(typeof(RB_Tools.Shared.Review.Properties.Level)).Length];

            // Keep track of the reviews we find
            var reviews = new List<CommitReview>();

            // Spin through them all and parse the log message
            foreach (SvnLogs.Log thisLog in svnLogs)
            {
                // Get the line with the review state in it
                string reviewStateLine = thisLog.Message.FirstOrDefault(line =>
                {
                    Match regExMatch = Regex.Match(line, @".*\[Review State.*:.*\]");
                    return regExMatch.Success;
                });

                // Do we have anything
                if (string.IsNullOrEmpty(reviewStateLine) == true)
                {
                    ++unknownReviews;
                    continue;
                }

                // Get the type of review
                try
                {
                    // Get the type of review string
                    string[] reviewTypeSplit = reviewStateLine.Split(':');
                    string reviewType = reviewTypeSplit[1].Replace("]", "").Trim();

                    // Find the review type this is
                    for (int i = 0; i < commitCounts.Length; ++i)
                    {
                        string thisEnumName = ((RB_Tools.Shared.Review.Properties.Level)i).GetSplitName();
                        if (thisEnumName.Equals(reviewType, StringComparison.InvariantCultureIgnoreCase) == true)
                        {
                            // If this is a review, pull out the review
                            if (i == (int)RB_Tools.Shared.Review.Properties.Level.FullReview)
                            {
                                string reviewUrl = GetReviewUrl(thisLog.Message);
                                if (string.IsNullOrEmpty(reviewUrl) == false)
                                    reviews.Add( new CommitReview(thisLog.Revision, reviewUrl) );
                            }

                            // Track the number of reviews
                            ++commitCounts[i];
                            break;
                        }
                    }
                }
                catch
                {
                    // Something has gone wrong
                    return null;
                }

            }

            // Return our reviews
            return new GetCommitStatsResult(commitCounts, unknownReviews, reviews.ToArray());
        }

        //
        // Returns the review URL from the commit
        //
        private static string GetReviewUrl(string[] message)
        {
            // Get the line with the review address in
            string serverNamePattern = string.Format(@".*{0}.*", Names.Url[(int)Names.Type.Reviewboard]);
            string reviewStateLine = message.FirstOrDefault(line =>
            {
                Match regExMatch = Regex.Match(line, serverNamePattern);
                return regExMatch.Success;
            });
            return (reviewStateLine == null ? null : reviewStateLine.Trim());
        }

        //
        // Gets the results of a review request
        //
        private static string GetUrlReviewState(string url, string workingDirectory, Simple credentials)
        {
            string reviewId = ExtractReviewId(url);
            string apiRequest = string.Format(@"/review-requests/{0}", reviewId);

            RB_Tools.Shared.Targets.Reviewboard.RequestApiResult result = RB_Tools.Shared.Targets.Reviewboard.RequestApi(credentials, apiRequest, workingDirectory);
            if (result.Result == null)
                throw new ReviewboardApiException(result.Error);
            




            return null;
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
    }
}



