using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Logging;
using System;
using System.Collections.Concurrent;
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
            int                         m_commitsWithoutJiras;
            int                         m_commitsWithJiras;

            Dictionary<string, UInt64>  m_validJiras;
            Dictionary<string, UInt64>  m_invalidJiras;

            public JiraStatistics(int noJiraCount, int withJiraCount, ConcurrentDictionary<string, UInt64> validJiras, ConcurrentDictionary<string, UInt64> invalidJiras)
            {
                m_commitsWithoutJiras = noJiraCount;
                m_commitsWithJiras = withJiraCount;

                m_validJiras = new Dictionary<string, ulong>(validJiras);
                m_invalidJiras = new Dictionary<string, ulong>(invalidJiras);
            }
        };

        //
        // Builds up stats about Jira usage
        //
        public static JiraStatistics GetJiraStatistics(SvnLogs.Log[] revisionLogs, Simple credentials, Logging logger, StatisticGenerated statsGenerated)
        {
            // Track our updates
            Object writeLock = new object();
            int revisionCount = 0;

            int commitsWithoutJira = 0;

            ConcurrentDictionary<string, UInt64> validJiras = new ConcurrentDictionary<string, ulong>();
            ConcurrentDictionary<string, UInt64> invalidJiras = new ConcurrentDictionary<string, ulong>();

            // We need to spin through every review and pull out the information about each one
            logger.Log("Starting to pull out the Jira stats from the given revision list");
            ParallelLoopResult result = Parallel.ForEach(revisionLogs, new ParallelOptions { MaxDegreeOfParallelism = 1 }, (thisRevision, loopState) =>
            {
                // Assume no Jira until we have one
                bool noJira = true;

                // Get the Jira line
                string jiraLine = thisRevision.Message.FirstOrDefault(line =>
                {
                    return line.Contains(@"[Jira Issue(s):");
                });

                // Do we have one?
                if (string.IsNullOrEmpty(jiraLine) == false && jiraLine.Contains(@"N/A") == false)
                {
                    // Pull out the Jira information
                    string[] splitIssuesLine = jiraLine.Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);

                    // We need two, the description and the Jira
                    if (splitIssuesLine.Length == 2)
                    {
                        // Ok, this is something we should be able to use...
                        // Get rid of the [] which might exist at the moment
                        string jirasOnly = splitIssuesLine[1].Replace("[", "").Replace("]", "").Replace(" ", "");
                        string[] individualJiras = jirasOnly.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        // Check the Jira is valid
                        foreach (string thisJira in individualJiras)
                        {
                            // Note we have a Jira now
                            noJira = false;

                            // Check if we track this as valid or invalid
                            bool isJiraValid = false;
                            try
                            {
                                isJiraValid = RB_Tools.Shared.Targets.Jira.ValidateJiraTicker(credentials, thisJira);
                            }
                            catch (Exception)
                            {
                                // It's not valid
                            }

                            // Get the dictionary and update it
                            ConcurrentDictionary<string, UInt64> dictionaryToUpdate = isJiraValid == true ? validJiras : invalidJiras;
                            dictionaryToUpdate.AddOrUpdate(thisJira, 1, (jira, count) =>
                            {
                                return ++count;
                            });
                        }
                    }
                }

                // Continue?
                if (loopState.IsStopped == false)
                {
                    // Update
                    lock (writeLock)
                    {
                        logger.Log("Updating Jira properties");

                        // Update our stats
                        if (noJira)
                            ++commitsWithoutJira;

                        // Update
                        statsGenerated(++revisionCount);
                    }
                }
            });

            return new JiraStatistics(commitsWithoutJira, revisionLogs.Length - commitsWithoutJira, validJiras, invalidJiras);
        }
    }
}
