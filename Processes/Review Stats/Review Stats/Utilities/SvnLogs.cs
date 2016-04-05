using RB_Tools.Shared.Utilities;
using Stats_Runner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Review_Stats.Utilities
{
    class SvnLogs
    {
        // Result of an SVN log request
        public class Log
        {
            public int      Revision { get; private set; }

            public string   Author { get; private set; }
            public DateTime Date { get; private set; }

            public string[] Message { get; private set; }

            // Constructor
            public Log(int revision, string[] log, string author, string date)
            {
                Revision = revision;
                Message = log;

                Author = author;

                // Pull the time apart so we can save it
                string dateFormat = @"yyyy-MM-dd HH:mm:ss";
                try
                {
                    // Format the date so we on;y save what we want
                    string[] timeSplit = date.Split('.');
                    string timeToFormat = timeSplit[0].Replace("T", " ");

                    // Save the time
                    Date = DateTime.ParseExact(timeToFormat, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Just a dummy time
                    Date = DateTime.ParseExact(@"2001-01-01 00:00:00", dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

        //
        // Gets the logs for a given set of revisions
        //
        public static Log[] GetRevisionLogs(string svnPath, string[] revisions)
        {
            // Spin through all the revisions requested
            List<Log> logs = new List<Log>();
            foreach(string thisRevision in revisions)
            {
                string logOutput = Svn.GetLog(svnPath, thisRevision, true);
                if (logOutput == null)
                    return null;

                // Read in the log input
                Log[] individualLogs = ParseLogOutput(logOutput);
                if (individualLogs == null)
                    return null;

                // Keep track
                logs.AddRange(individualLogs);
            }

            // Return all our logs
            return logs.ToArray();
        }

        //
        // Spilts up the log message into individual revisions if the output included more than 1 revision
        //
        private static Log[] ParseLogOutput(string logOutput)
        {
            // Load the XML in and read in the content
            List<Log> logs = new List<Log>();
            bool parsed = Xml.LoadXml(logOutput, (XmlDocument xmlDocument) =>
            {
                // Root
                XmlElement logRoot = xmlDocument.DocumentElement;

                // Get all the log entries
                XmlNodeList logEntries = logRoot.SelectNodes("logentry");
                if (logEntries.Count == 0)
                    return false;

                // Spin through them all
                logs.Capacity = logEntries.Count;
                foreach (XmlNode thisLogEntry in logEntries)
                {
                    int revision = int.Parse(thisLogEntry.Attributes[@"revision"].Value);
                    string author = thisLogEntry.SelectSingleNode(@"author").InnerText;
                    string date = thisLogEntry.SelectSingleNode(@"date").InnerText;
                    string message = thisLogEntry.SelectSingleNode(@"msg").InnerText;

                    // Create and add this log
                    Log thisLog = new Log(
                        revision,
                        message.Split('\n'),
                        author,
                        date);
                    logs.Add(thisLog);
                }


                return true;
            });
            if (parsed == false)
                return null;

            // Return our content, put it in the right order
            logs.Reverse();
            return logs.ToArray();
        }
    }
}
