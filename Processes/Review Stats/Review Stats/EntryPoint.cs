using RB_Tools.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Review_Stats
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Set up
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RB_Tools.Shared.Cli.Options commandLineOptions = new RB_Tools.Shared.Cli.Options();
            if (CommandLine.Parser.Default.ParseArguments(args, commandLineOptions))
            {
                // Create our logger
                Logging.Type loggingType = (commandLineOptions.Logging == true ? Logging.Type.File : Logging.Type.Null);
                Logging logger = Logging.Create("Generate Review Stats", loggingType, Logging.Threading.MultiThread);

                // Run the dialog
                Application.Run(new Dialogs.Progress(commandLineOptions.FileList, commandLineOptions.InjectPaths, logger));
            }
        }
    }
}
