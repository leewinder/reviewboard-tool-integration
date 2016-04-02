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
            Application.Run(new Dialogs.SimpleAuthentication(RB_Tools.Shared.Server.Names.Url[(int)RB_Tools.Shared.Server.Names.Type.Reviewboard], Reviewboard.Authenticate));
        }
    }
}
