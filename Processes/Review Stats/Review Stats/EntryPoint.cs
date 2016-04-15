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

            // Check we have some arguments
            if (args == null || args.Length == 0)
            {
                // Show an error dialog
                MessageBox.Show("No arguments have been passed to the statistics dialog so no statistics can be generated", "Unable To Generate Statistics", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Run the dialog
            Application.Run(new Dialogs.Progress(args[0], args.Length < 2 ? null : args[1]));
        }
    }
}
