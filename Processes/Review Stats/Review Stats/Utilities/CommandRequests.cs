using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RB_Tools.Shared.Utilities;
using RB_Tools.Shared.Logging;

namespace Review_Stats.Utilities
{
    // Builds up the command requests
    class CommandRequests
    {
        // Results of the command parse
        public class Result
        {
            public string[]     ValidPaths { get; private set; }
            public string[]     InvalidPaths { get; private set; }

            // Constructor
            public Result(string[] validPaths, string[] invalidPaths)
            {
                ValidPaths = validPaths;
                InvalidPaths = invalidPaths;
            }
        }

        //
        // Parses the command line options
        //
        public static Result ParseCommands(string fileList, bool injectPaths, Logging logger)
        {
            logger.Log(@"Parsing the file list command");

            // Get the file
            if (File.Exists(fileList) == false)
            {
                logger.Log(@"Unable to find the file list command file - {0}", fileList);
                return null;
            }

            string[] fileContent = File.ReadAllLines(fileList);
            if (fileContent.Length == 0)
            {
                logger.Log(@"The given file list contains no content - {0}", fileList);
                return null;
            }

            // Handle any debug options
            fileContent = HandleDebugRequest(fileContent, injectPaths);

            // Track all our content
            List<string> validPaths = new List<string>();
            List<string> invalidPaths = new List<string>();

            // Check they all exist, and check they all all SVN repositories
            foreach (string thisPath in fileContent)
            {
                // Even a string?
                if (string.IsNullOrWhiteSpace(thisPath) == true)
                    continue;

                // Does it exist
                if (File.Exists(thisPath) == false && Directory.Exists(thisPath) == false)
                {
                    logger.Log(@"* Invalid path found in file list - {0}", thisPath);
                    invalidPaths.Add(thisPath);
                    continue;
                }

                // SVN path
                if (Svn.IsPathTracked(thisPath) == false)
                {
                    logger.Log(@"* Invalid path found in file list - {0}", thisPath);
                    invalidPaths.Add(thisPath);
                    continue;
                }

                // It's fine
                logger.Log(@"* Using - {0}", thisPath);
                validPaths.Add(thisPath);
            }

            // Return our paths
            return new Result(validPaths.ToArray(), invalidPaths.ToArray());
        }

        // Properties
        private static readonly string InjectDirectoryRequest = "inject_test_path";

        //
        // Updates the data based on the debug options
        //
        private static string[] HandleDebugRequest(string[] fileContent, bool injectPaths)
        {
            // If we need to, inject the test path
            if (injectPaths == true)
            {
                // Get the path we need to inject into the test documents
                string runningPath = Directory.GetCurrentDirectory();

                // Removed the working copy
                string injectionPath = Path.GetDirectoryName(runningPath);
                injectionPath = Path.GetDirectoryName(injectionPath);
                injectionPath = Paths.Clean(injectionPath);

                // Spin through the entries and inject the directory so our paths are relative
                fileContent = fileContent.Select(x =>
                {
                    // Remove the root and clean up
                    x = string.Format(x, injectionPath);
                    x = Paths.Clean(x);

                    return x;
                }).ToArray();
            }

            // Return our content
            return fileContent;
        }


    }
}
