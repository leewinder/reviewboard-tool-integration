using RB_Tools.Shared.Extensions;
using RB_Tools.Shared.Server;
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
        // Results of commit review types
        public class GetCommitStatsResult
        {
            public int[]    CommitCount { get; private set; }
            public int      UnknownCommits { get; private set; }

            public string[] Reviews { get; private set; }

            public GetCommitStatsResult(int[] commitCount, int unknown, string[] reviews)
            {
                CommitCount = commitCount;
                UnknownCommits = unknown;

                Reviews = reviews;
            }
        };

        // Results of a review stats request
        public class GetReviewStatisticsResult
        {

        }

        //
        // Returns the statistics on the raised reviews
        //
        public static GetReviewStatisticsResult GetReviewStatistics(string[] reviewList)
        {
            return null;
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
            List<string> reviews = new List<string>();

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
                                    reviews.Add(reviewUrl);
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
    }
}



