using RB_Tools.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Create_Review
{
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RB_Tools.Shared.Cli.Options commandLineOptions = new RB_Tools.Shared.Cli.Options();
            if (CommandLine.Parser.Default.ParseArguments(args, commandLineOptions))
            {
                // Create our logger
                Logging.Type loggingType = (commandLineOptions.Logging == true ? Logging.Type.File : Logging.Type.Null);
                Logging logger = Logging.Create("Create Review", loggingType, Logging.Threading.MultiThread);

                // Copy user settings from previous version if necessary
                UpdateVersionSettings();

                // Build up the content we've been asked to review
                // If we don't have any, we've already complained
                Review.Review.Content requestContent = Review.Review.ExtractContent(commandLineOptions.SkipReviews, commandLineOptions.FileList, commandLineOptions.InjectPaths, logger);
                if (requestContent == null)
                    return;

                // Run the dialog
                Application.Run(new CreateReview(commandLineOptions.FileList, requestContent, logger));
            }
        }

        //
        // Updates the settings from a previous version if needed
        //
        private static void UpdateVersionSettings()
        {
            // If we need to update, do it
            if (Settings.Settings.Default.UpdateNeeded == true)
            {
                // Update our settings
                Settings.Settings.Default.Upgrade();

                // Make sure we don't do it again
                Settings.Settings.Default.UpdateNeeded = false;
                Settings.Settings.Default.Save();
            }
        }
    }
}
