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
        public static Revisions[] Request(string[] pathList)
        {
            // Spin through and get them all
            List<Revisions> results = new List<Revisions>();
            foreach (string thisPath in pathList)
            {
                Revisions thisResult = Request(thisPath);
                if (thisResult != null)
                    results.Add(thisResult);
            }

            // Return our list
            return (results.Count == 0 ? null : results.ToArray());
        }

        //
        // Returns a revision list for a given path
        //
        private static Revisions Request(string path)
        {
            // Get the list of revisions we want to look at
            string revisonList = GetRevisionList(path);
            if (revisonList == null)
                return null;

            // Parse the list of revisions we're interested in
            string[] revisionsToReview = ParseRevisionList(revisonList);
            if (revisionsToReview == null)
                return null;

            // Get the URL of this repository
            string url = Svn.GetBranch(path);

            // Return our results
            return new Revisions(path, url, revisionsToReview);
        }

        //
        // Gets a list of revision from Tortoise SVN
        //
        private static string GetRevisionList(string path)
        {
            // Get the path we'll write our revisions to
            string tempPath = Path.GetTempFileName();

            try
            {
                string commandLineOptions = string.Format(@"/command:log /path:""{0}"" /outfile:""{1}""", path, tempPath);
                Process.Start(null, "tortoiseproc", commandLineOptions);

                // Pull out the data
                string[] revisionList = File.ReadAllLines(tempPath);

                // Lose the temp file
                File.Delete(tempPath);

                // Return the list of files
                return (revisionList.Length == 0 ? null : revisionList[0]);
            }
            catch
            {
                // This just means we can;t find anything
                return null;
            }
        }

        // 
        // Parses the list of revisions we need to look at, and returns the whole set of numbers
        //
        private static string[] ParseRevisionList(string revisonList)
        {
            // Split up the revisions
            string[] individualRevisions = revisonList.Split(',');
            if (individualRevisions.Length == 0)
                return null;

            // Add them to the list, making sure they are in the right format
            List<string> revisions = new List<string>();
            foreach (string thisRevision in individualRevisions)
            {
                if (string.IsNullOrWhiteSpace(thisRevision) == true)
                    continue;

                // Make sure the format is right
                string formattedRevision = thisRevision.Replace('-', ':');
                revisions.Add(formattedRevision);
            }

            // Return our list - in accending order
            revisions.Reverse();
            return (revisions.Count == 0 ? null : revisions.ToArray());
        }

    }
}
