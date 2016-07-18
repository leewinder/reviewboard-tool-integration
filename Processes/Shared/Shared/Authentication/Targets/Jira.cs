using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Authentication.Targets
{
    // Authentication for a Jira Server
    public class Jira
    {
        //
        // Starts authenticating against the Jira server
        //
        public static Credentials.Credentials Authenticate(Logging.Logging logger)
        {
            // Get our data
            string serverUrl = Server.Names.Url[(int)Server.Names.Type.Jira];
            string serverName = Server.Names.Type.Jira.ToString();

            logger.Log("Starting authentication");
            logger.Log(" * Name: {0}", serverName);
            logger.Log(" * Url: {0}", serverUrl);

            // Kick off the RB authentation
            Dialogs.SimpleAuthentication authDialog = new Dialogs.SimpleAuthentication(serverName, serverUrl, Jira.Authenticate, logger);
            authDialog.ShowDialog();

            // Load in our credentials object
            Credentials.Credentials rbCredentials = Credentials.Credentials.Create(serverUrl, logger);
            return rbCredentials;

        }

        //
        // Authenticates against the given Jira server
        //
        private static Result Authenticate(string server, string username, string password, Logging.Logging logger)
        {
            // Check we have the properties we need
            bool validServer = string.IsNullOrWhiteSpace(server) == false;
            bool validUser = string.IsNullOrWhiteSpace(username) == false;
            bool validPassword = string.IsNullOrWhiteSpace(password) == false;

            // If it's not valid, throw and we're done
            if (validServer == false || validUser == false || validPassword == false)
            {
                logger.Log("Invalid server, username or password requested");
                throw new ArgumentException("Invalid server, username or password requested");
            }

            return new Result(false, "Jira authentication not supported");
        }
    }
}
