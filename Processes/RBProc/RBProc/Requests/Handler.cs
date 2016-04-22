using RB_Tools.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace RBProc.Requests
{
    class Handler
    {
        //
        // Constructor
        //
        public Handler(string process, string requestPath)
        {
            // Trim any trailing seperators from the path
            requestPath = requestPath.TrimEnd(Path.DirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar).Trim(Path.PathSeparator);

            // Save our properties
            m_processRequest = process;
            m_requestPath = requestPath;
        }

        //
        // Handles this request
        //
        public void Process()
        {
            // Check the given file exists
            bool fileExists = DoesRequestedFileExist();
            if (fileExists == false)
                return;

            // Get the request object we can use
            Utilities.Cache rbRequests = new Utilities.Cache(m_processRequest, m_requestPath);

            // Own this process to stop any other process writing to the file
            bool firstProcess = false;
            using (Utilities.ProcessLock rbProcess = new Utilities.ProcessLock())
            {
                // Create and write to the shared file
                firstProcess = rbRequests.CreateRequestFile();
                rbRequests.WriteRequest();
            }

            // If this was the first instance, we now need to wait until all other process have finished
            if (firstProcess == true)
            {
                // Get the application to deal with this command
                string requiredApplication = GetRequestProcessApplication(m_processRequest);
                if (string.IsNullOrWhiteSpace(requiredApplication) == true)
                    return;

                // Since this is the main app, we now wait to make sure we've caught them all
                string commandLineProperties = string.Empty;
                while (string.IsNullOrEmpty(commandLineProperties) == true)
                {
                    // Wait until we need to the request file
                    Thread.Sleep(DelayBeforeCheckingRequests);
                    if (RequestsReadyToBeProcessed(rbRequests) == false)
                        continue;

                    // We can read in the file now
                    commandLineProperties = GetCommandLinePropertiesFile(rbRequests);
                    if (string.IsNullOrWhiteSpace(commandLineProperties) == true)
                        return;
                }

                // Build up the command line options
                string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string finalExecutable = string.Format("{0}\\{1}", applicationPath, requiredApplication);

                // Run the executable and bail
                RunProcess(finalExecutable, commandLineProperties);
            }
        }

        // Request applications
        private static readonly Dictionary<string, string> ProcessApplications = new Dictionary<string, string>()
        {
            {@"create_new_review", @"Create Review.exe"},
            {@"settings", @"Settings.exe"},
            {@"review_svn_repo", @"Review Stats.exe"},
            {@"open_about_dialog", @"About.exe"},
            {@"open_rb_portal", @"open_browser.bat"},
        };

        // Time check properties
        private const int DelayBeforeCheckingRequests = 500;
        private const int AcceptablePeriodSinceLastWrite = 300;

        // Request properties
        private readonly string m_processRequest;
        private readonly string m_requestPath;

        //
        // Returns if the requested file actually exists
        //
        private bool DoesRequestedFileExist()
        {
            try
            {
                File.GetAttributes(m_requestPath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception when trying to get the attributes of the requested file\n\n" + e.Message, @"Unable to run RBProc");
                return false;
            }
        }

        //
        // Returns if enough time has passed to read the file
        //
        private static bool RequestsReadyToBeProcessed(Utilities.Cache rbRequests)
        {
            // Pull out the last modified timestamp and see if enough time has passed
            Int64 modifiedTimeStamp = rbRequests.GetModifiedTimeStamp(true);
            Int64 currentTimeStamp = Stopwatch.GetTimestamp();

            Int64 elapsedTicks = currentTimeStamp - modifiedTimeStamp;
            Int64 elapsedMilliseconds = (elapsedTicks / Stopwatch.Frequency) * 1000;

            // It needs to have been a fixed time before we read the file
            return (elapsedMilliseconds >= AcceptablePeriodSinceLastWrite);
        }

        //
        // Returns the application we need to process this request
        //
        private static string GetRequestProcessApplication(string requestedProcess)
        {
            // Get the application to deal with this command
            try
            {
                string requiredApplication = ProcessApplications[requestedProcess];
                return requiredApplication;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to find the executable to handle the process request '" + requestedProcess + "'\n" + e.Message, @"Unable to run RBProc");
                return null;
            }
        }

        //
        // Creates the command line properties file we'll use
        //
        private static string GetCommandLinePropertiesFile(Utilities.Cache rbRequests)
        {
            // Pull out the requests we've recieved
            string[] completedRequest = rbRequests.RetriveRequests(true);
            if (completedRequest.Length == 0)
            {
                MessageBox.Show(@"No requests were provided for the process", @"Unable to run RBProc");
                return null;
            }

            // Build up the parameters to pass
            try
            {
                // Build the file we'll use
                string commandLineProperties = Path.GetTempFileName();
                File.WriteAllLines(commandLineProperties, completedRequest);

                // Pass back the file
                return commandLineProperties;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to generate the process command properties\n" + e.Message, @"Unable to run RBProc");
                return null;
            }
        }

        //
        // Starts a process and reads out the std output and error
        //
        public static void RunProcess(string command, string commandLineProperties)
        {
            // Load our settings
            Options settings = Settings.Load();

            // Build up the command line options
            string options = string.Format(@"--file-list {0}", commandLineProperties);
            if (settings.EnableLogging == true)
                options += @" --enable-logging";

            // Build up the process
            ProcessStartInfo processInfo = new ProcessStartInfo(command, options);

            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            // Start the process
            System.Diagnostics.Process.Start(processInfo);
        }
    }
}
