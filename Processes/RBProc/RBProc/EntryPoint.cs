using System;
using System.Windows.Forms;

namespace RBProc
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Before we do anything, we need to arguments[request, path]
            if (args == null || args.Length != 2)
            {
                MessageBox.Show(@"Given arguments were not expected", @"Unable to run RBProc");
                return;
            }

            // Pull out our properties
            string requestedProcess = args[0];
            string requestPath = args[1];

            // Pass it through
            Requests.Handler handler = new Requests.Handler(requestedProcess, requestPath);
            handler.Process();
        }
    }
}
