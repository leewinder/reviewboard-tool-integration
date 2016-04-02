using RB_Tools.Shared.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RB_Tools.Shared.Authentication.Credentials
{
    public abstract class Credentials
    {
        //
        // Creates a credentials object from an existing server file
        //
        public static Credentials Create(string server)
        {
            // Get the path to these credentials
            string filePath = GetServerFilePath(server);
            if (File.Exists(filePath) == false)
                return null;

            // Load in the data
            string[] credentialData = File.ReadAllLines(filePath);

            // Convert the data from the credentials file into it's own array
            string[] fileData = new string[credentialData.Length-2];
            for (int i = 0; i < fileData.Length; ++i)
                fileData[i] = credentialData[i + 2];

            // Create our object
            Type thisType = Type.GetType("RB_Tools.Shared.Authentication.Credentials." + credentialData[0]);
            Credentials credentials = (Credentials)Activator.CreateInstance(thisType, fileData);

            // Set our server
            credentials.Server = credentialData[1];

            // We have an object
            return credentials;
        }

        //
        // Creates a new credentials file for a given server
        //
        public static Credentials Create(string server, string user, string password)
        {
            // Encrypt the password 
            password = Cipher.Encrypt(password, Identifiers.UUID);
            StoreCredentials(typeof(Simple).Name, server, new string[] { user, password });

            // Load the credentials in
            Credentials loadedCredentials = Create(server);
            return loadedCredentials;
        }

        //
        // Clears an existing credentials object
        //
        public static Credentials Clear(string server)
        {
            string filePath = GetServerFilePath(server);
            if (File.Exists(filePath) == true)
                File.Delete(filePath);

            // Load the credentials in
            Credentials loadedCredentials = Create(server);
            return loadedCredentials;
        }

        // Shared information
        public string Server { get; private set; }

        // Private properties
        private const string CredentialsFolder = @"Credentials";

        //
        // Saves a given credentials file
        //
        private static void StoreCredentials(string type, string server, string[] data)
        {
            // Get the file path
            string path = GetServerFilePath(server);

            // Write our the data
            using (StreamWriter file = new StreamWriter(path))
            {
                // Write out the header info
                file.WriteLine(type);
                file.WriteLine(server);

                // Write out the specific data
                foreach(string thisLine in data)
                    file.WriteLine(thisLine);
            }

        }

        //
        // Returns the filename of a given server
        //
        private static string GetServerFile(string server)
        {
            return server.Replace('/', '_').Replace(':', '_').Replace('.', '_');
        }

        //
        // Returns the path of the server file
        //
        private static string GetServerFilePath(string server)
        {
            // Make sure the credentials folder exists
            string credentialsFolder = string.Format(@"{0}\{1}", Paths.GetDataFolder(), CredentialsFolder);
            Directory.CreateDirectory(credentialsFolder);

            // Get the file name
            string fileName = GetServerFile(server);
            string path = string.Format(@"{0}\{1}", credentialsFolder, fileName);

            return path;
        }
    }
}
