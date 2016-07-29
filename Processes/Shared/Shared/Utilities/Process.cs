using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Utilities
{
    public class Process
    {
        public class Output
        {
            public readonly string StdOut;
            public readonly string StdErr;

            public Output(string stdOut, string stdErr)
            {
                StdOut = stdOut;
                StdErr = stdErr;
            }
        };

        //
        // Starts a process and reads out the std output and error
        //
        public static Output Start(string workingDirectory, string command, string options)
        {
            // Build up the process
            ProcessStartInfo processInfo = new ProcessStartInfo(command, options);

            // Redirect std
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            if (string.IsNullOrWhiteSpace(workingDirectory) == false)
                processInfo.WorkingDirectory = workingDirectory;

            // Start the process
            System.Diagnostics.Process process = null;
            try
            {
                process = System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("Unable to run {0} with {1}\n\n{2}", command, options, e.Message);
                return new Output(string.Empty, errorMessage);
            }

            // Read the output
            string output, error;
            using (System.IO.StreamReader stdOut = process.StandardOutput)
            {
                output = stdOut.ReadToEnd();
            }
            using (System.IO.StreamReader stdErr = process.StandardError)
            {
                error = stdErr.ReadToEnd();
            }

            // Send it back
            return new Output(output, error);
        }
    }
}
