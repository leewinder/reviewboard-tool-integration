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
        public static void KeepAssets()
        {
            // If we have a folder, we can just leave
            if (string.IsNullOrEmpty(storageFolder) == false)
                return;

            // Get the folder we'll use
            string myDocsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string thisReviewFolder = DateTime.Now.ToString("yy.MM.dd.hh.mm.ss");

            // Build up the folder we'll use
            storageFolder = string.Format("{0}/Reviewboard Integration Tools/Reviews/{1}/", myDocsFolder, thisReviewFolder);
            Directory.CreateDirectory(storageFolder);
        }

        //
        // Keeps the folder passed through
        //
        public static bool Keep(string file, string newName, bool removeOriginal)
        {
            // Only if we're set up
            if (string.IsNullOrEmpty(storageFolder) == false)
            {
                // Get the final name
                string newNameToUse = newName;
                if (string.IsNullOrEmpty(newName) == false)
                    newNameToUse = newName;

                // Copy the folder over
                try { File.Copy(file, storageFolder + newNameToUse, true); } catch (Exception) { }
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
