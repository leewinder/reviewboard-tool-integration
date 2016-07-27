using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Review_Stats.Utilities
{
    //
    // State of Jira within this review period
    //
    class JiraState
    {
        // Public Delegates
        public delegate void StatisticGenerated(int count);

        // Stats regarding Jira usage in this period
        public class JiraStatistics
        {

        };

        //
        // Builds up stats about Jira usage
        //
        public static JiraStatistics GetJiraStatistics(SvnLogs.Log[] revisionLogs, Simple credentials, Logging logger, StatisticGenerated statsGenerated)
        {
            // Track our updates
            Object writeLock = new object();
            int revisionCount = 0;

            // We need to spin through every review and pull out the information about each one
            logger.Log("Starting to pull out the Jira stats from the given revision list");
            ParallelLoopResult result = Parallel.ForEach(revisionLogs, new ParallelOptions { MaxDegreeOfParallelism = 16 }, (thisRevision, loopState) =>
            {
                // Continue?
                if (loopState.IsStopped == false)
                {
                    // Update
                    lock (writeLock)
                    {
                        logger.Log("Updating Jira properties");

                        // Update
                        statsGenerated(++revisionCount);
                    }
                }
            });

            return new JiraStatistics();
        }
    }
}
