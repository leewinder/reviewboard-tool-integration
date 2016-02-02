using System.Diagnostics;

namespace Create_Review.Utilities
{
    //
    // Support functions for process starts
    //
    class Process
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
                processInfo.WorkingDirectory = workingDirectory ;

            // Start the process
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(processInfo);

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
