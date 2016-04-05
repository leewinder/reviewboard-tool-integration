using RB_Tools.Shared.Extensions;
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
        public class GetReviewStatsResults
        {
            public int[]    CommitCount { get; private set; }
            public int      UnknownCommits { get; private set; }

            public GetReviewStatsResults(int[] commitCount, int unknown)
            {
                CommitCount = commitCount;
                UnknownCommits = unknown;
            }
        };

        //
        // Counts the type of reviews in a set of commits
        //
        public static GetReviewStatsResults GetReviewStats(SvnLogs.Log[] svnLogs)
        {
            // We need to track the types of reviews
            int unknownReviews = 0;
            int[] commitCounts = new int[Enum.GetNames(typeof(RB_Tools.Shared.Review.Properties.Level)).Length];

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
            return new GetReviewStatsResults(commitCounts, unknownReviews);
        }
    }
}



