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
using Newtonsoft.Json;
using RB_Tools.Shared.Logging;

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
            public SvnLogs.Log  Log { get; private set; }
            public string       Review { get; private set; }

            // Constructor
            public CommitReview(SvnLogs.Log log, string review)
            {
                Log = log;
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
        public static GetCommitStatsResult GetCommitStatistics(SvnLogs.Log[] svnLogs, Logging logger, StatisticGenerated statGenerated)
        {
            // We need to track the types of reviews
            Object  writeLock = new Object();
            int     logCount = 0;
            int     unknownReviews = 0;
            int[]   commitCounts = new int[Enum.GetNames(typeof(RB_Tools.Shared.Review.Properties.Level)).Length];

            // Keep track of the reviews we find
            var reviews = new List<CommitReview>();

            // Spin through them all and parse the log message
            logger.Log("Starting to pull out the commit statistics from the SVN logs");
            ParallelLoopResult result = Parallel.ForEach(svnLogs, new ParallelOptions { MaxDegreeOfParallelism = 16 }, (thisLog, loopState) =>
            {
                // Get the line with the review state in it
                string reviewStateLine = thisLog.Message.FirstOrDefault(line =>
                {
                    Match regExMatch = Regex.Match(line, @".*\[Review State.*:.*\]");
                    return regExMatch.Success;
                });

                // Identify what we have
                logger.Log(@"* Found the review state line '{0}' in revision {1}", reviewStateLine, thisLog.Revision);

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
                                // Let us know what type of review we found
                                logger.Log(@"* Found review type '{0}'", ((RB_Tools.Shared.Review.Properties.Level)i).ToString());

                                // If this is a review, pull out the review
                                if (i == (int)RB_Tools.Shared.Review.Properties.Level.FullReview)
                                {
                                    string reviewUrl = GetReviewUrl(thisLog.Message);
                                    if (string.IsNullOrEmpty(reviewUrl) == false)
                                        commitToAdd = new CommitReview(thisLog, reviewUrl);
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
                        // Update
                        logger.Log("Updating the review type counts");

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
            {
                logger.Log("The commit statistics completed unsuccessfully");
                return null;
            }

            // Update what we found
            logger.Log("The following review types were found");
            logger.Log("* Commit count: {0}", commitCounts);
            logger.Log("* Unknown reviews: {0}", unknownReviews);
            for (int i = 0; i < reviews.Count; ++i)
                logger.Log("* {0}: {1}", ((RB_Tools.Shared.Review.Properties.Level)i).ToString(), reviews[i]);

            // Return our reviews
            return new GetCommitStatsResult(commitCounts, unknownReviews, reviews.ToArray());
        }

        //
        // Returns the statistics on the raised reviews
        //
        public static ReviewStatistics[] GetReviewStatistics(CommitReview[] reviewList, string workingDirectory, Simple credentials, Logging logger, StatisticGenerated statGenerated)
        {
            // Track our updates
            Object  writeLock = new object();
            int     reviewCount = 0;
            var     properties = new List<ReviewStatistics>(reviewList.Length);

            // We need to spin through every review and pull out the information about each one
            logger.Log("Starting to pull out the review stats from generated reviews");
            ParallelLoopResult result = Parallel.ForEach(reviewList, new ParallelOptions { MaxDegreeOfParallelism = 16 }, (thisReview, loopState) =>
            {
                // Get our properties
                ReviewStatistics reviewProperties = GetPropertiesOfReview(thisReview, workingDirectory, credentials, logger);
                if (reviewProperties == null)
                    loopState.Stop();

                // Continue?
                if (loopState.IsStopped == false)
                {
                    // Update
                    lock (writeLock)
                    {
                        logger.Log("Updating review properties");

                        // Update
                        properties.Add(reviewProperties);
                        statGenerated(++reviewCount);
                    }
                }
            });

            // How did we do?
            if (result.IsCompleted == false)
            {
                logger.Log("The review statistics completed unsuccessfully");
                return null;
            }

            // Return our properties
            logger.Log("Created {0} review stats", properties.Count);
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
        private static ReviewStatistics GetPropertiesOfReview(CommitReview review, string workingDirectory, Simple credentials, Logging logger)
        {
            // Get the API
            string reviewId = ExtractReviewId(review.Review);
            string apiRequest = string.Format(@"/review-requests/{0}", reviewId);

            // Try this review code
            JObject result = MakeApiCall(credentials, workingDirectory, apiRequest, logger);
            if (result == null)
            {
                logger.Log("API called failed, returning no statistics");
                return new ReviewStatistics(review, State.NotFound);
            }

            // If it's an empty review it's pending because we couldn't access
            if (result == EmptyReview)
            {
                logger.Log("API called failed with Empty Review, assuming a pending review");
                return new ReviewStatistics(review, State.Pending);
            }

            // We have the review information so process
            Tuple<int, int, int> reviewsAndShipitsAndReplies = GetReviewsAndShipItsAndReplies(workingDirectory, reviewId, credentials, logger);

            // Get the state of this review
            State reviewState = GetReviewState(result, logger);

            // Return our properties
            return new ReviewStatistics(review, reviewState, reviewsAndShipitsAndReplies.Item1, reviewsAndShipitsAndReplies.Item3, reviewsAndShipitsAndReplies.Item2);
        }
        
        //
        // Returns the number of reviews and replies
        //
        private static Tuple<int, int, int> GetReviewsAndShipItsAndReplies(string workingDirectory, string reviewId, Simple credentials, Logging logger)
        {
            int reviews = 0;
            int shipIts = 0;
            JObject reviewResponse = null;

            try
            {
                logger.Log("Requesting reviews and replies");

                // Try this review code
                string apiRequest = string.Format(@"/review-requests/{0}/reviews", reviewId);
                reviewResponse = MakeApiCall(credentials, workingDirectory, apiRequest, logger);
                if (reviewResponse == null)
                {
                    logger.Log("API called, assumed no reviews or replies");
                    return new Tuple<int, int, int>(0, 0, 0);
                }
                
                // Get the reviews from the call
                string reviewCount = (string)reviewResponse["total_results"];
                reviews = int.Parse(reviewCount);

                // Spin through and get the number of ship-its
                JArray reviewInfo = (JArray)reviewResponse["reviews"];
                if (reviewInfo != null)
                {
                    dynamic reviewArray = reviewInfo.ToObject<dynamic>();
                    foreach (dynamic thisReview in reviewArray)
                    {
                        if (thisReview.ship_it == true)
                            ++shipIts;
                    }
                }
            }
            catch (Exception e)
            {
                // Can't get the values out
                logger.Log("Exception raised when requesting reviews\n\n{0}\n", e.Message);
                return new Tuple<int, int, int>(0, 0, 0);
            }

            // If we have no reviews, bail
            if (reviews == 0)
            {
                logger.Log("No reviews were found");
                return new Tuple<int, int, int>(0, 0, 0);
            }

            // We now need to track replies
            int replies = 0;

            // Get the replies in this response
            JArray commentList = (JArray)reviewResponse["reviews"];
            Parallel.ForEach(commentList.Children(), new ParallelOptions { MaxDegreeOfParallelism = 16 }, (thisComment, loopState) =>
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
                    JObject result = MakeApiCall(credentials, workingDirectory, apiRequest, logger);

                    // If we failed, return what we have
                    if (result == null)
                    {
                        logger.Log("API called failed, assumed no replies");
                        loopState.Stop();
                    }

                    if (loopState.IsStopped == false)
                    {
                        string replyCount = (string)result["count"];
                        Interlocked.Add(ref replies, int.Parse(replyCount));
                    }
                }
                catch (Exception e)
                {
                    // Can't get the values out so return what we have
                    logger.Log("Exception raised when requesting replies\n\n{0}\n", e.Message);
                    loopState.Stop();
                }
            });

            // Return what we have
            logger.Log("Found\n* Reviews: {0}\n* Ship Its: {1}\n* Replies: {2}", reviews, shipIts, replies);
            return new Tuple<int, int, int>(reviews, shipIts, replies);
        }

        //
        // Gets the state of this review
        //
        private static State GetReviewState(JObject result, Logging logger)
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
            catch (Exception e)
            {
                // Unknown state
                logger.Log("Exception raised when checking review status\n\n{0}\n", e.Message);
                return State.Unknown;
            }

            // Done
            return thisState;
        }

        //
        // Runs through the RB Api
        //
        private static JObject MakeApiCall(Simple credentials, string workingDirectory, string apiRequest, Logging logger)
        {
            // Make the API request
            logger.Log("Requesting RB API call - {0}", apiRequest);
            RB_Tools.Shared.Targets.Reviewboard.RequestApiResult result = RB_Tools.Shared.Targets.Reviewboard.RequestApi(credentials, apiRequest, workingDirectory);

            // We can deal with a non-success call in a numver of ways
            if (result.Code == RB_Tools.Shared.Targets.Reviewboard.Result.Error)
            {
                logger.Log("* Error thrown - {0}", result.Error);
                throw new ReviewboardApiException(result.Error);
            }

            // If we're unable to access the review, return an empty one
            if (result.Code == RB_Tools.Shared.Targets.Reviewboard.Result.AccessDenied)
            {
                logger.Log("* Access denied");
                return EmptyReview;
            }

            // If we can't find it, return null as we want to track this
            if (result.Code != RB_Tools.Shared.Targets.Reviewboard.Result.Success)
            {
                logger.Log("* Failed");
                return null;
            }

            // Return our object
            logger.Log("* Result - {0}", result.Result);
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



