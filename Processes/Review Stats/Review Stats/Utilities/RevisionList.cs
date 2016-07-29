using RB_Tools.Shared.Logging;
using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Review_Stats.Utilities
{
    // Gets the revsion list to review
    class RevisionList
    {
        // Result of a call to get the revisions
        public class Revisions
        {
            public string Path { get; private set; }
            public string Url { get; private set; }

            public string[] Revision { get; private set; }

            // Constructor
            public Revisions(string path, string url, string[] revisions)
            {
                Path = path;
                Url = url;

                Revision = revisions;
            }
        }

        //
        // Gets the revision lists for a list of files
        //
        public static Revisions[] Request(string[] pathList, Logging logger)
        {
            // Spin through and get them all
            List<Revisions> results = new List<Revisions>();
            foreach (string thisPath in pathList)
            {
                logger.Log(@"Finding revisions for {0}", thisPath);
                Revisions thisResult = Request(thisPath, logger);
                if (thisResult != null)
                    results.Add(thisResult);
            }

            // Return our list
            return (results.Count == 0 ? null : results.ToArray());
        }

        //
        // Returns a revision list for a given path
        //
        private static Revisions Request(string path, Logging logger)
        {
            // Get the list of revisions we want to look at
            string revisonList = GetRevisionList(path, logger);
            if (revisonList == null)
            {
                logger.Log("No revisions were provided");
                return null;
            }

            // Parse the list of revisions we're interested in
            string[] revisionsToReview = ParseRevisionList(revisonList, logger);
            if (revisionsToReview == null)
                return null;

            // Get the URL of this repository
            string url = Svn.GetBranch(path);
            logger.Log("Using SVN URL '{0}', url");

            // Return our results
            return new Revisions(path, url, revisionsToReview);
        }

        //
        // Gets a list of revision from Tortoise SVN
        //
        private static string GetRevisionList(string path, Logging logger)
        {
            // Get the path we'll write our revisions to
            string tempPath = Path.GetTempFileName();

            try
            {
                string commandLineOptions = string.Format(@"/command:log /strict /path:""{0}"" /outfile:""{1}""", path, tempPath);
                logger.Log("* Running TortoiseProc command '{0}'", commandLineOptions);

                // Run the process which will result in the output file
                Process.Start(null, "tortoiseproc", commandLineOptions);
                string[] revisionList = File.ReadAllLines(tempPath);

                // Display what we found
                logger.Log("* Found the following revisions - {0}", (revisionList.Length == 0 ? "none" : revisionList[0]));

                // Lose the temp file
                File.Delete(tempPath);

                // Return the list of files
                return (revisionList.Length == 0 ? null : revisionList[0]);
            }
            catch (Exception e)
            {
                // This just means we can't find anything
                logger.Log("Error generated when trying to find revision list\n\\{0}\n", e.Message);
                return null;
            }
        }

        // 
        // Parses the list of revisions we need to look at, and returns the whole set of numbers
        //
        private static string[] ParseRevisionList(string revisonList, Logging logger)
        {
            logger.Log("Parsing revision list - ", revisonList);

            // Split up the revisions
            string[] individualRevisions = revisonList.Split(',');
            if (individualRevisions.Length == 0)
            {
                logger.Log("* No revisions were found as a result of the ',' split");
                return null;
            }

            // Add them to the list, making sure they are in the right format
            List<string> revisions = new List<string>();
            foreach (string thisRevision in individualRevisions)
            {
                if (string.IsNullOrWhiteSpace(thisRevision) == true)
                    continue;

                // Identify what we have found
                logger.Log("* Found revision {0}", thisRevision);

                // Make sure the format is right
                string formattedRevision = thisRevision.Replace('-', ':');
                revisions.Add(formattedRevision);
            }

            // How many did we find?
            logger.Log("* Found {0} revision(s)", revisions.Count);

            // Return our list - in accending order
            revisions.Reverse();
            return (revisions.Count == 0 ? null : revisions.ToArray());
        }

    }
}
