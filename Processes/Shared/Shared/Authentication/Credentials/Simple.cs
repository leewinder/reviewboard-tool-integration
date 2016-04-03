using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Authentication.Credentials
{
    // Simple username/password credentials object
    class Simple : Credentials
    {
        // Properties
        public string   User { get; private set; }
        public string   Password { get; private set; }

        // Constructor
        public Simple(string user, string password)
        {
            // Read in the data
            User = user;
            Password = Cipher.Decrypt(password, Identifiers.UUID);
        }
    }
}
