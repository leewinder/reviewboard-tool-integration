using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create_Review.Utilities
{
    //
    // Path utilities
    //
    class Paths
    {
        public enum Type
        {
            Directory,
            File,
            None,
        };

        //
        // Cleans the given path
        //
        public static string Clean(string path)
        {
            path = path.Trim(Path.DirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar).Trim(Path.PathSeparator);
            return path;
        }

        //
        // Returns the type of path
        //
        public static Type GetType(string path)
        {
            try
            {
                // Get the properties of this source
                FileAttributes attr = File.GetAttributes(path);

                if (attr.HasFlag(FileAttributes.Directory) == true)
                    return Type.Directory;
                else
                    return Type.File;
                
            }
            catch (Exception)
            {

            }

            // If we get here it's not valid
            return Type.None;
        }
    }
}
