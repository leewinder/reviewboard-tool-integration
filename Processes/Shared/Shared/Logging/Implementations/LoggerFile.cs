using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Logging.Implementations
{
    class LoggerFile : Logging
    {
        //
        // Constructor
        //
        public LoggerFile(string logName, Threading threadBehaviour)
        {
            // Create our stream
            CreateStream(logName);

            // Set the right callback
            if (threadBehaviour == Threading.SingleThread)
                WriteText = WriteToFile;
            else
                WriteText = WriteToFileWithLock;
        }

        // Private properties
        private object          m_writeLock = new object();
        private StreamWriter    m_streamWriter = null;

        //
        // Writes to the file and pushes it out
        //
        private void WriteToFileWithLock(string logMessage)
        {
            // Lock then write
            lock(m_writeLock)
                WriteToFile(logMessage);
        }

        //
        // Writes to the file and pushes it out
        //
        private void WriteToFile(string logMessage)
        {
            if (string.IsNullOrEmpty(logMessage) == false)
            {
                string timePreFix = DateTime.Now.ToString("HH:mm:ss");
                string outputMessage = string.Format(@"{0} - {1}", timePreFix, logMessage);

                m_streamWriter.WriteLine(outputMessage);
                System.Diagnostics.Debug.Print(outputMessage);
            }
            else
            {
                m_streamWriter.WriteLine("");
                System.Diagnostics.Debug.Print("");
            }
        }

        //
        // Creates the stream
        //
        private void CreateStream(string logName)
        {
            // Create the directory
            string rootPath = string.Format(@"{0}\Logs", Paths.GetDataFolder());
            Directory.CreateDirectory(rootPath);

            // Get the file name
            string timePostFile = DateTime.Now.ToString("yy-MM-dd HH-mm-ss");
            string fileName = string.Format(@"{0} {1}.txt", logName, timePostFile);
            string logPath = string.Format(@"{0}\{1}", rootPath, fileName);

            m_streamWriter = new StreamWriter(logPath);
            m_streamWriter.AutoFlush = true;
        }

        //
        // Dispose method
        //
        protected override void Dispose(bool supressFinalise)
        {
            if (m_streamWriter != null)
                m_streamWriter.Dispose();
        }
    }
}
