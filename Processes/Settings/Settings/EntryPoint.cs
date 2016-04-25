using RB_Tools.Shared.Authentication.Targets;
using RB_Tools.Shared.Logging;
using System;
using System.Windows.Forms;

namespace Settings
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            RB_Tools.Shared.Cli.Options commandLineOptions = new RB_Tools.Shared.Cli.Options();
            if (CommandLine.Parser.Default.ParseArguments(args, commandLineOptions))
            {
                // Create our logger
                Logging.Type loggingType = (commandLineOptions.Logging == true ? Logging.Type.File : Logging.Type.Null);
                Logging logger = Logging.Create("Settings", loggingType, Logging.Threading.MultiThread);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Dialogs.SettingOptions(logger));
            }
        }
    }
}
