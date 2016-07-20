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

            // Create our rest client
            Atlassian.Jira.Jira jiraInstance = null;
            try
            {
                jiraInstance = Atlassian.Jira.Jira.CreateRestClient(server, username, password);
            }
            catch (Exception e)
            {
                string message = string.Format("Unable to generate Jira instance for user '{0}'\n\n{1}", username, e.InnerException.Message);

                logger.Log(message);
                throw new InvalidOperationException(message);
            }

            // Check we can access this user
            try
            {
                // Pull out the user
                Task<Atlassian.Jira.JiraUser> thisUser = jiraInstance.Users.GetUserAsync(username);
                thisUser.Wait();

                // If we get here, the user exists and is fine
            }
            catch (Exception)
            {
                string message = string.Format("Unable to access Jira user '{0}'\n\nIf your username and password is correct, it is likely you have been locked out of your account.\n\nPlease visit {1} to unlock your account and then try again", username, server);

                logger.Log(message);
                throw new InvalidOperationException(message);
            }

            // Return the result
            return new Result(true, "Authentication succeeded");
        }
    }
}
