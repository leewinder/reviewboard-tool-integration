using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace RBProc.Utilities
{
    //
    // Caches the requests between processes
    //
    class Cache
    {
        //
        // Constructor
        //
        public Cache(string process, string path)
        {
            // Save our properties
            m_processName = process;
            m_requestPath = path;
        }

        //
        // Creates the memory file request
        //
        public bool CreateRequestFile()
        {
            // Check we don't already have one
            if (m_masterMemoryFile != null)
                throw new InvalidOperationException(@"Master cache file has already been created within this process");

            // Check if this file already exists
            string mmfName = BuildFileName(m_processName);
            try
            {
                // If we can open this, we have nothing to create
                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName);
                return false;
            }
            catch (FileNotFoundException)
            {
                // This means we need it so just keep quiet
            }
            catch (Exception e)
            {
                // This is a problem
                throw e;
            }

            // Create the file we'll write to
            m_masterMemoryFile = MemoryMappedFile.CreateNew(mmfName, MemoryFileSizeInBytes);
            if (m_masterMemoryFile == null)
                throw new NullReferenceException("Unable to create the cache file for RBProc");

            // Done, we control this
            return true;
        }

        //
        // Writes the given request to the sared memory file
        //
        public void WriteRequest()
        {
            // Get our memory mapped file
            string mmfName = BuildFileName(m_processName);
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName))
            {
                // Create a view that starts are the end of the current written content
                using (MemoryMappedViewStream stream = mmf.CreateViewStream(0, 0))
                {
                    // Write the new timestamp
                    byte[] timeStamp = CreateNewTimeStamp();
                    stream.Write(timeStamp, 0, timeStamp.Length);

                    // Move along to the next section we can write to
                    int fileOffset = GetFileOffset();
                    stream.Seek(fileOffset, SeekOrigin.Begin);

                    // Write the path to the file
                    using (StreamWriter writer = new StreamWriter(stream))
                        writer.WriteLine(m_requestPath);
                }
            }
        }

        //
        // Reads in the requests from all the other processes
        //
        public string[] RetriveRequests(bool exclusiveAccess)
        {
            // Gain access again
            if (exclusiveAccess == true)
                AquireMutex();

            // Get the list of our files
            List<string> requests = new List<string>();

            // Get our memory mapped file
            string mmfName = BuildFileName(m_processName);
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream(sizeof(Int64), 0))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string thisline = null;
                        while ((thisline = reader.ReadLine()) != null)
                        {
                            // Due to how memory mapped files work, we do end up with a load of empty data
                            String lineToAdd = thisline.Trim(MemoryFillFiller);
                            if (string.IsNullOrWhiteSpace(lineToAdd) == false)
                            {
                                // Only add it if it doesn't exist
                                bool stringExists = requests.Exists(element => element.Equals(lineToAdd, StringComparison.InvariantCultureIgnoreCase) == true);
                                if (stringExists == false)
                                    requests.Add(thisline);
                            }
                        }
                    }
                }
            }

            // Done again
            if (exclusiveAccess == true)
                ReleaseMutex();

            // Return our requests
            return requests.ToArray();
        }

        //
        // Returns the last time the file was modified
        //
        public Int64 GetModifiedTimeStamp(bool exclusiveAccess)
        {
            // Gain access again
            if (exclusiveAccess == true)
                AquireMutex();

            // We need to track our timestamp
            Int64 timeStamp = 0;

            // Pull out the timestamp
            string mmfName = BuildFileName(m_processName);
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    byte[] timeStampData = new byte[sizeof(Int64)];
                    stream.Read(timeStampData, 0, timeStampData.Length);

                    // Convert over our timestamp
                    timeStamp = BitConverter.ToInt64(timeStampData, 0);
                }
            }

            // Done again
            if (exclusiveAccess == true)
                ReleaseMutex();

            // Return our time stamp
            return timeStamp;
        }

        // Class Properties
        private const string        MemoryMutexName = @"process-lock-rbproc-cache-mutex-c314f80d-2471-415d-8604-62fe74ba1a3e";
        private const string        MemoryFileName = @"process-lock-rbproc-cache-file-{0}-c314f80d-2471-415d-8604-62fe74ba1a3e";

        private const long          MemoryFileSizeInBytes = 1 * 1024 *1024;

        static readonly char[]      MemoryFillFiller = new char[] { '\0' };
        static readonly string      MemoryLineEnd = "\r\n";

        // Private Properties
        private Mutex               m_processMutex = null;
        private MemoryMappedFile    m_masterMemoryFile = null;

        private readonly string     m_processName;
        private readonly string     m_requestPath;


        //
        // Builds up the name of the memory mapped file
        //
        private string BuildFileName(string process)
        {
            string fileName = string.Format(MemoryFileName, process);
            return fileName;
        }

        //
        // Blocks on the mutex
        //
        private void AquireMutex()
        {
            m_processMutex = new System.Threading.Mutex(false, MemoryMutexName);
            m_processMutex.WaitOne();
        }

        //
        // Loses the mutex
        //
        private void ReleaseMutex()
        {
            m_processMutex.ReleaseMutex();
            m_processMutex = null;
        }

        //
        // Returns the offset required given the content of the file
        //
        private int GetFileOffset()
        {
            // Get the content of the file at the moment
            string[] filePaths = RetriveRequests(false);

            // Build up the entire string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string thisEntry in filePaths)
                stringBuilder.Append(thisEntry).Append(MemoryLineEnd);

            // Return the length of the content (including the time stamp)
            return stringBuilder.Length + sizeof(Int64);
        }

        //
        // Returns the timestamp to be written to the map file
        //
        private byte[] CreateNewTimeStamp()
        {
            Int64 timeStamp = Stopwatch.GetTimestamp();
            return BitConverter.GetBytes(timeStamp);
        }
    }
}
