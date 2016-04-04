using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Utilities
{
    //
    // SVN utilities
    //
    public class Svn
    {
        //
        // Gets the root path for this repository
        //
        public static string GetRoot(string path)
        {
            // Clean the path
            path = Paths.Clean(path);

            // Check it's not invalid
            Paths.Type type = Paths.GetType(path);
            if (type == Paths.Type.None)
                return null;

            // If it's a file, get the directory
            if (type == Paths.Type.File)
                path = Path.GetDirectoryName(path);

            // Run the svn info command on this path
            string commandOption = string.Format("info \"{0}\"", path);
            Process.Output svnOutput = Process.Start(null, "svn", commandOption);

            // If we have errors, we can't use it
            if (string.IsNullOrWhiteSpace(svnOutput.StdErr) == false)
                return null;

            // We have some properties, so pull out what we're interested in
            string[] parsedOutput = svnOutput.StdOut.Split('\n');
            foreach (string line in parsedOutput)
            {
                string lineToFind = "Working Copy Root Path:";

                // If we can find it, remove it and return
                if (line.StartsWith(lineToFind) == true)
                    return line.Remove(0, lineToFind.Length).Trim();
            }

            // Can't find the root
            return null;
        }

        //
        // Returns the current branch of SVN
        //
        public static string GetBranch(string workingDirectory)
        {
            // Generate the info
            Process.Output infoOutput = Process.Start(workingDirectory, "svn", "info");
            if (string.IsNullOrWhiteSpace(infoOutput.StdOut) == true)
                return null;

            // Find the URL
            string[] output = infoOutput.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string thisLine in output)
            {
                string trimmedLine = thisLine.Trim();
                if (trimmedLine.StartsWith("URL: ") == true)
                {
                    string url = trimmedLine.Replace("URL: ", "");
                    return url;
                }
            }

            // Got here so it's not here
            return null;
        }

        //
        // Returns if the given path is under SVN control
        //
        public static bool IsPathTracked(string path)
        {
            // Clean the path
            path = Paths.Clean(path);

            // Folder or file?
            Paths.Type pathType = Paths.GetType(path);
            if (pathType == Paths.Type.None)
                return false;

            // Check for a folder or file
            if (pathType == Paths.Type.Directory)
                return IsFolderTracked(path);
            else
                return IsFileTracked(path);
        }

        //
        // Returns if the given folder is tracked
        //
        private static bool IsFolderTracked(string path)
        {
            string commandOption = string.Format("info \"{0}\"", path);
            Process.Output svnOutput = Process.Start(null, "svn", commandOption);

            // If we have output it's valid
            return (string.IsNullOrWhiteSpace(svnOutput.StdOut) == false);
        }

        //
        // Returns if the given path is tracked
        //
        private static bool IsFileTracked(string file)
        {
            string filePath = Path.GetDirectoryName(file);

            // List the files present in this root path
            string commandOption = string.Format("status --depth files \"{0}\"", filePath);
            Process.Output svnOutput = Process.Start(null, "svn", commandOption);

            // If we have errors, it's not tracked
            if (string.IsNullOrWhiteSpace(svnOutput.StdOut) == true)
                return false;

            // Break up the output and see if we can find this entry
            string[] fileList = svnOutput.StdOut.Split('\n');
            foreach (string thisFile in fileList)
            {
                // Clean it up
                string lineToCheck = thisFile.Trim();

                // Does this file name exist?
                if (lineToCheck.EndsWith(file, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    // It's in the list, if it's unknown then it's not tracked
                    if (lineToCheck.StartsWith("?") == true)
                        return false;
                    else
                        return true;
                }
            }

            // If we get here it's not tracked
            return false;
        }
    }
}
