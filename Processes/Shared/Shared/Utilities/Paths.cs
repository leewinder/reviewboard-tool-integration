using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Utilities
{
    public class Paths
    {
        //
        // Returns the data folder for the tools
        //
        public static string GetDataFolder()
        {
            // Get our tools folder
            string applicationFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string toolsFolder = string.Format(@"{0}\Reviewboard Integration Tools", applicationFolder);

            // Make sure it exists
            Directory.CreateDirectory(toolsFolder);

            // Return the path
            return toolsFolder;
        }
    }
}
