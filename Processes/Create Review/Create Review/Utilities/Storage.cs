using RB_Tools.Shared.Logging;
using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create_Review.Utilities
{
    //
    // Stores the review artifacts when needed
    //
    class Storage
    {
        //
        // Enables storge of assets, if this isn't called then other calls are noop
        //
        public static void KeepAssets(string summary, Logging logger)
        {
            // If we have a folder, we can just leave
            if (string.IsNullOrEmpty(storageFolder) == false)
                return;

            // Get the folder we'll use
            string myDocsFolder = Paths.GetDocumentsFolder();

            // Build up the folder we'll use
            storageFolder = string.Format("{0}/Reviews/{1}/", myDocsFolder, summary);
            logger.Log("Storing content in '{0}'", storageFolder);

            // Make sure we're not replacing one
            if (Directory.Exists(storageFolder) == true)
                Directory.Delete(storageFolder, true);

            // Make sure it's there
            Directory.CreateDirectory(storageFolder);
        }

        //
        // Keeps the folder passed through
        //
        public static bool Keep(string file, string newName, bool removeOriginal, Logging logger)
        {
            // Only if we're set up
            if (string.IsNullOrEmpty(storageFolder) == false)
            {
                // Get the final name
                string newNameToUse = newName;
                if (string.IsNullOrEmpty(newName) == false)
                    newNameToUse = newName;

                // Copy the folder over
                try
                {
                    File.Copy(file, storageFolder + newNameToUse, true);
                    logger.Log("Storing content in '{0}{1}'", storageFolder, newNameToUse);
                }
                catch (Exception)
                {
                }
            }

            // Remove the original if we need to
            if (removeOriginal == true)
                try { File.Delete(file); } catch (Exception) { }

            // Done, return if we saved or not
            return string.IsNullOrEmpty(storageFolder) == false;
        }

        // Folder used to store the assets
        private static string storageFolder = string.Empty;
    }
}
