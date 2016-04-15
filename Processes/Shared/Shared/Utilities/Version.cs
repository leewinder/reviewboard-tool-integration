using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Utilities
{
    public class Version
    {
        // Properties
        public static readonly string VersionNumber;
        public static readonly string BuildNumber;

        //
        // Static constructor
        //
        static Version()
        {
            // Pull out the version numbers
            string[] versionNumbers = GetVersionAndBuildNumbers();
            VersionNumber = versionNumbers != null ? versionNumbers[0] : "Unknown";
            BuildNumber = versionNumbers != null ? versionNumbers[1] : "Unknown";
        }

        //
        // Gets the current version and build numbers
        //
        private static string[] GetVersionAndBuildNumbers()
        {
            // Get where we are
            string executaionLocation = Assembly.GetExecutingAssembly().Location;
            string execuationPath = Path.GetDirectoryName(executaionLocation);

            // Get the version content
            string[] versionNumbers = null;
            try
            {
                versionNumbers = File.ReadAllLines(execuationPath + "\\..\\version");
            }
            catch (Exception)
            {
                return null;
            }

            // Check we have two entries with actual content
            if (versionNumbers.Length != 2)
                return null;

            // Check we have two entries with actual content
            if (string.IsNullOrWhiteSpace(versionNumbers[0]) == true || string.IsNullOrWhiteSpace(versionNumbers[1]) == true)
                return null;

            // Return what we have
            return versionNumbers;
        }
    }
}
