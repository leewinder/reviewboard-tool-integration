using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Targets
{
    // Shared information about the RB target
    public class Reviewboard
    {
        //
        // Returns the path to the reviewboard executable
        //
        public static string Path()
        {
            // Do we already have it
            if (string.IsNullOrWhiteSpace(RbtPath) == false)
                return RbtPath;

            // Get the path
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, "where", "rbt");

            // Break up the paths given
            string[] pathsReturned = output.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string thisPath in pathsReturned)
            {
                if (thisPath.ToLower().Contains("rbt.cmd") == true)
                    RbtPath = thisPath;

            }

            // Could we find it?
            if (string.IsNullOrWhiteSpace(RbtPath) == true)
                throw new System.ArgumentNullException("Unable to find the rbt.cmd path");

            // Done
            return RbtPath;
        }

        // Private properties
        private static string RbtPath { get; set; }
    }
}
