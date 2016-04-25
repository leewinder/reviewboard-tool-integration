using RB_Tools.Shared.Logging;
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
        // Public Delegates
        public delegate void LogRetrieved(int count);

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
        public static Log[] GetRevisionLogs(string svnPath, string[] revisions, Logging logger, LogRetrieved logRetrieved)
        {
            // We need to track properties throughout the loop
            Object      writeLock = new Object();
            List<Log>   logs = new List<Log>();
            int         logCount = 0;

            // Spin through all the revisions requested
            ParallelLoopResult result = Parallel.ForEach(revisions, new ParallelOptions { MaxDegreeOfParallelism = 16 }, (thisRevision, loopState) =>
            {
                // Pull out the log
                string logOutput = Svn.GetLog(svnPath, thisRevision, true, logger);
                if (logOutput == null)
                    loopState.Stop();

                // Get the log we found
                logger.Log("* Recieved log\n{0}\n", logOutput);

                // Continue?
                if (loopState.IsStopped == false)
                {
                    // Read in the log input
                    Log[] individualLogs = ParseLogOutput(logOutput, logger);
                    if (individualLogs == null)
                        loopState.Stop();

                    // How many did we get
                    logger.Log("* Identified {0} logs", individualLogs.Length);

                    // Continue?
                    if (loopState.IsStopped == false)
                    {
                        // Lock our writes
                        lock (writeLock)
                        {
                            // Add and update
                            logs.AddRange(individualLogs);
                            logRetrieved(++logCount);
                        }
                    }
                }
            });

            // If we didn't succeed, bail
            if (result.IsCompleted == false)
            {
                logger.Log("* The log generation loop did not complete successfully");
                return null;
            }

            // Return all our logs
            logger.Log("* In total we found {0} logs", logs.Count);
            return logs.ToArray();
        }

        //
        // Spilts up the log message into individual revisions if the output included more than 1 revision
        //
        private static Log[] ParseLogOutput(string logOutput, Logging logger)
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
                {
                    logger.Log("Unable to find a 'logentry' XML node");
                    return false;
                }

                try
                {
                    // Spin through them all
                    logs.Capacity = logEntries.Count;
                    foreach (XmlNode thisLogEntry in logEntries)
                    {
                        int revision = int.Parse(thisLogEntry.Attributes[@"revision"].Value);
                        string author = thisLogEntry.SelectSingleNode(@"author").InnerText;
                        string date = thisLogEntry.SelectSingleNode(@"date").InnerText;
                        string message = thisLogEntry.SelectSingleNode(@"msg").InnerText;

                        // Output what we found
                        logger.Log("* Log found:\n  * {0}\n  * {1}\n  * {2}\n  * {3}", revision, author, date, message);

                        // Create and add this log
                        Log thisLog = new Log(
                            revision,
                            message.Split('\n'),
                            author,
                            date);
                        logs.Add(thisLog);
                    }
                }
                catch (Exception e)
                {
                    logger.Log("Exception raised when attempting to parse the SVN log message\n\n{0}\n", e.Message);
                    return false;
                }


                return true;
            });

            // How did we do?
            if (parsed == false)
            {
                logger.Log("Unable to parse the given XML log message");
                return null;
            }

            // Let ppl know what we have
            logger.Log("Managed to parse {0} log messages", logs.Count);

            // Return our content, put it in the right order
            logs.Reverse();
            return logs.ToArray();
        }
    }
}
