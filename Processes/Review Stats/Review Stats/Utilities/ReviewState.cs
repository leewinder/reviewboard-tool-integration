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
using Newtonsoft.Json.Linq;
using RB_Tools.Shared.Structures;
using System.Threading;

namespace Review_Stats.Utilities
{
    // Provides information about reviews
    class ReviewState
    {
        // Public Delegates
        public delegate void StatisticGenerated(int count);

        // Review states
        public enum State
        {
            Pending, 

            Open,
            Closed,

            Discarded,
            NotFound,

            Unknown,
        }

        // A commit with an associated review
        public class CommitReview
        {
            public int      Revision { get; private set; }
            public string   Review { get; private set; }

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

        // Statistics of a given review
        public class ReviewStatistics
        {
            public CommitReview Commit { get; private set; }
            public State        State { get; private set; }

            public int          Reviews { get; private set; }
            public int          Replies { get; private set; }

            public int          ShipIts { get; private set; }

            // Constructor
            public ReviewStatistics(CommitReview commit, State state, int reviews, int replies, int shipIts)
            {
                Commit = commit;

                State = state;

                Reviews = reviews;
                Replies = replies;

                ShipIts = shipIts;
            }

            // Constructor
            public ReviewStatistics(CommitReview commit, State state) : this(commit, state, 0, 0, 0)
            {
            }
        }

        //
        // Counts the type of reviews in a set of commits
        //
        public static GetCommitStatsResult GetCommitStatistics(SvnLogs.Log[] svnLogs, StatisticGenerated statGenerated)
        {
            // We need to track the types of reviews
            Object  writeLock = new Object();
            int     logCount = 0;
            int     unknownReviews = 0;
            int[]   commitCounts = new int[Enum.GetNames(typeof(RB_Tools.Shared.Review.Properties.Level)).Length];

            // Keep track of the reviews we find
            var reviews = new List<CommitReview>();

            // Spin through them all and parse the log message
            ParallelLoopResult result = Parallel.ForEach(svnLogs, (thisLog, loopState) =>
            {
                // Get the line with the review state in it
                string reviewStateLine = thisLog.Message.FirstOrDefault(line =>
                {
                    Match regExMatch = Regex.Match(line, @".*\[Review State.*:.*\]");
                    return regExMatch.Success;
                });

                // Do we have anything
                bool unknownReview = false;
                if (string.IsNullOrEmpty(reviewStateLine) == true)
                    unknownReview = true;

                // Get the type of review if we need to
                int commitType = -1;
                CommitReview commitToAdd = null;
                if (unknownReview == false)
                {
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
                                        commitToAdd = new CommitReview(thisLog.Revision, reviewUrl);
                                }

                                // Track the number of reviews
                                commitType = i;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // Something has gone wrong
                        loopState.Stop();
                    }
                }

                // Only bother if we need to
                if (loopState.IsStopped == false)
                {
                    // Update our information
                    lock (writeLock)
                    {
                        // Update the view type
                        if (unknownReview == true)
                            ++unknownReviews;
                        else if (commitType >= 0)
                            ++commitCounts[commitType];

                        // Store if needed
                        if (commitToAdd != null)
                            reviews.Add(commitToAdd);

                        // Another one done
                        statGenerated(++logCount);
                    }
                }
            });

            // Did we fail 
            if (result.IsCompleted == false)
                return null;

            // Return our reviews
            return new GetCommitStatsResult(commitCounts, unknownReviews, reviews.ToArray());
        }

        //
        // Returns the statistics on the raised reviews
        //
        public static ReviewStatistics[] GetReviewStatistics(CommitReview[] reviewList, string workingDirectory, Simple credentials, StatisticGenerated statGenerated)
        {
            // Track our updates
            Object  writeLock = new object();
            int     reviewCount = 0;
            var     properties = new List<ReviewStatistics>(reviewList.Length);

            // We need to spin through every review and pull out the information about each one
            ParallelLoopResult result = Parallel.ForEach(reviewList, (thisReview, loopState) =>
            {
                // Get our properties
                ReviewStatistics reviewProperties = GetPropertiesOfReview(thisReview, workingDirectory, credentials);
                if (reviewProperties == null)
                    loopState.Stop();

                // Continue?
                if (loopState.IsStopped == false)
                {
                    // Update
                    lock (writeLock)
                    {
                        properties.Add(reviewProperties);
                        statGenerated(++reviewCount);
                    }
                }
            });

            // How did we do?
            if (result.IsCompleted == false)
                return null;

            // Return our properties
            return properties.ToArray();
        }

        // Private Properties
        private static readonly JObject EmptyReview = new JObject();

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
        private static ReviewStatistics GetPropertiesOfReview(CommitReview review, string workingDirectory, Simple credentials)
        {
            // Get the API
            string reviewId = ExtractReviewId(review.Review);
            string apiRequest = string.Format(@"/review-requests/{0}", reviewId);

            // Try this review code
            JObject result = MakeApiCall(credentials, workingDirectory, apiRequest);
            if (result == null)
                return new ReviewStatistics(review, State.NotFound);

            // If it's an empty review it's pending because we couldn't access
            if (result == EmptyReview)
                return new ReviewStatistics(review, State.Pending);

            // We have the review information so process
            int shipItCount = GetShipItCount(result);
            Pair<int, int> reviewsAndReplies = GetReviewsAndReplies(workingDirectory, reviewId, credentials);

            // Get the state of this review
            State reviewState = GetReviewState(result);

            // Return our properties
            return new ReviewStatistics(review, reviewState, reviewsAndReplies.First, reviewsAndReplies.Second, shipItCount);
        }

        //
        // Returns the number of ship it's in a review
        //
        private static int GetShipItCount(JObject result)
        {
            try
            {
                string shipItCount = (string)result["review_request"]["ship_it_count"];
                return int.Parse(shipItCount);
            }
            catch
            {
                // Nothing
                return 0;
            }
        }

        //
        // Returns the number of reviews and replies
        //
        private static Pair<int, int> GetReviewsAndReplies(string workingDirectory, string reviewId, Simple credentials)
        {
            int reviews = 0;
            JObject reviewResponse = null;
            try
            {
                // Try this review code
                string apiRequest = string.Format(@"/review-requests/{0}/reviews", reviewId);
                reviewResponse = MakeApiCall(credentials, workingDirectory, apiRequest);
                if (reviewResponse == null)
                    return new Pair<int, int>();
                
                // Get the reviews from the call
                string reviewCount = (string)reviewResponse["total_results"];
                reviews = int.Parse(reviewCount);
            }
            catch
            {
                // Can't get the values out
                return new Pair<int, int>();
            }

            // If we have no reviews, bail
            if (reviews == 0)
                return new Pair<int, int>();

            // We now need to track replies
            int replies = 0;

            // Get the replies in this response
            JArray commentList = (JArray)reviewResponse["reviews"];
            Parallel.ForEach(commentList.Children(), (thisComment, loopState) =>
            {
                // Do we need to?
                if (loopState.IsStopped == true)
                    return;

                try
                {
                    // Pull out the ID of this comment so we can see the replies
                    string commentId = (string)thisComment["id"];

                    // Build up the request
                    string apiRequest = string.Format(@"/review-requests/{0}/reviews/{1}/replies --counts-only=1", reviewId, commentId);
                    JObject result = MakeApiCall(credentials, workingDirectory, apiRequest);

                    // If we failed, return what we have
                    if (result == null)
                        loopState.Stop();

                    if (loopState.IsStopped == false)
                    {
                        string replyCount = (string)result["count"];
                        Interlocked.Add(ref replies, int.Parse(replyCount));
                    }
                }
                catch
                {
                    // Can't get the values out so return what we have
                    loopState.Stop();
                }
            });

            // Return what we have
            return new Pair<int, int>(reviews, replies);
        }

        //
        // Gets the state of this review
        //
        private static State GetReviewState(JObject result)
        {
            State thisState = State.Unknown;
            try
            {
                // Get the state of this review
                string currentState = (string)result["review_request"]["status"];
                string publicState = (string)result["review_request"]["public"];

                // Check our state
                if (currentState.Equals("pending", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    if (publicState.Equals("true", StringComparison.InvariantCultureIgnoreCase) == true)
                        thisState = State.Open;
                    else
                        thisState = State.Pending;
                }
                else if (currentState.Equals("discarded", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    thisState = State.Discarded;
                }
                else if (currentState.Equals("submitted", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    thisState = State.Closed;
                }
            }
            catch
            {
                // Unknown state
                return State.Unknown;
            }

            // Done
            return thisState;
        }

        //
        // Runs through the RB Api
        //
        private static JObject MakeApiCall(Simple credentials, string workingDirectory, string apiRequest)
        {
            // Make the API request
            RB_Tools.Shared.Targets.Reviewboard.RequestApiResult result = RB_Tools.Shared.Targets.Reviewboard.RequestApi(credentials, apiRequest, workingDirectory);

            // We can deal with a non-success call in a numver of ways
            if (result.Code == RB_Tools.Shared.Targets.Reviewboard.Result.Error)
                throw new ReviewboardApiException(result.Error);

            // If we're unable to access the review, return an empty one
            if (result.Code == RB_Tools.Shared.Targets.Reviewboard.Result.AccessDenied)
                return EmptyReview;

            // If we can't find it, return null as we want to track this
            if (result.Code != RB_Tools.Shared.Targets.Reviewboard.Result.Success)
                return null;

            // Return our object
            return result.Result;
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



