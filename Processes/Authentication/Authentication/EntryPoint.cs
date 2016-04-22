using RB_Tools.Shared.Authentication.Targets;
using RB_Tools.Shared.Logging;
using System;
using System.Windows.Forms;

namespace Authentication
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
            Logging logger = Logging.Create("Authentication", Logging.Type.File, Logging.Threading.MultiThread);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Dialogs.CredentialOptions(logger));
        }
    }
}
