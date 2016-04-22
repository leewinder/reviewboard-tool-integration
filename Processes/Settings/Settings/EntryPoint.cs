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
        static void Main()
        {
            // Enable logging
            Logging logger = Logging.Create("Settings", Logging.Type.File, Logging.Threading.MultiThread);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Dialogs.SettingOptions(logger));
        }
    }
}
