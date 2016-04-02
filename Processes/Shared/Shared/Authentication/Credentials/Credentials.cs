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
            return null;
        }

        //
        // Creates a new credentials file for a given server
        //
        public static Credentials Create(string server, string user, string password)
        {
            return null;
        }

        //
        // Clears an existing credentials object
        //
        public static Credentials Clear(string server)
        {
            return null;
        }


        public string Server { get; private set; }
    }
}
