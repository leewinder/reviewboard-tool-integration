using RB_Tools.Shared.Authentication.Credentials;
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
        public static JiraStatistics GetJiraStatistics(SvnLogs.Log[] revisionLogs, Simple credentials, StatisticGenerated statsGenerated)
        {
            return null;
        }
    }
}
