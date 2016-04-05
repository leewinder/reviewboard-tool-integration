using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Review_Stats.Utilities
{
    class SvnLogs
    {
        // Result of an SVN log request
        public class Result
        {
            public int Revision { get; private set; }
            public string[] Log { get; private set; }

            // Constructor
            public Result(int revision, string[] log)
            {
                Revision = revision;
                Log = log;
            }
        }

        //
        // Gets the logs for a given set of revisions
        //
        public static Result[] GetRevisionLogs(string svnPath, string[] revisions)
        {
            // Spin through all the revisions requested
            List<Result> logs = new List<Result>();
            foreach(string thisRevision in revisions)
            {
                string[] logOutput = Svn.GetLog(svnPath, thisRevision);
                if (logOutput == null)
                    return null;

                // Parse the log so we have a log per revision
                SplitLogsResult revisionLogs = SplitCombinedLogs(logOutput, thisRevision);

                // Create an object for each log message
                for (int i = 0; i < revisionLogs.Logs.Count; ++i)
                {
                    Result thisResult = new Result(revisionLogs.StartRevision + i, revisionLogs.Logs[i]);
                    logs.Add(thisResult);
                }

            }


            return logs.ToArray();
        }

        // Result of splitting up a set of logs
        private class SplitLogsResult
        {
            public int              StartRevision { get; private set; }
            public List<string[]>   Logs { get; private set; }

            public SplitLogsResult(int startRevision, List<string[]> logs)
            {
                StartRevision = startRevision;
                Logs = logs;
            }
        }

        //
        // Spilts up the log message into individual revisions if the output included more than 1 revision
        //
        private static SplitLogsResult SplitCombinedLogs(string[] logOutput, string thisRevision)
        {
            // Break up a sequence of revisions
            string[] revisions = thisRevision.Split(':');

            // Get the revision we start from
            int startRevision;
            bool parsedRevision = int.TryParse(revisions[0], out startRevision);
            if (parsedRevision == false)
                return null;

            // Is this just a single revision
            if (revisions.Length == 1)
            {
                List<string[]> revisionLog = new List<string[]>() { logOutput };
                return new SplitLogsResult(startRevision, revisionLog);
            }

            // We have multiple logs, so split them up
            List<string[]> revisionLog2 = new List<string[]>() { logOutput };
            return new SplitLogsResult(startRevision, revisionLog2);
        }
    }
}
