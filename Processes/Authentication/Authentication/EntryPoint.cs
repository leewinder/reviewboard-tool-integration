using RB_Tools.Shared.Authentication.Targets;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Dialogs.CredentialOptions());
        }
    }
}
