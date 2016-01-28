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

            // Check we have some arguments
            if (args == null || args.Length == 0)
            {
                // Show an error dialog
                MessageBox.Show("No arguments have been passed to the review dialog so no review can be raised", "Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Build up the content we've been asked to review
            // If we don't have any, we've already complained
            Utilities.Review.Content requestContent = Utilities.Review.ExtractContent(args[0], args.Length < 2 ? null : args[1]);
            if (requestContent == null)
                return;

            // Run the dialog
            Application.Run(new CreateReview(requestContent));
        }
    }
}
