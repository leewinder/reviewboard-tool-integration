using System;

namespace RB_Tools.Shared.Authentication.Targets
{
    // Authentication for a Reviewboard Server
    public class Reviewboard
    {
        //
        // Starts authenticating against the Reviewboard server
        //
        public static Credentials.Credentials Authenticate(Logging.Logging logger)
        {
            // Get our data
            string serverUrl = Server.Names.Url[(int)Server.Names.Type.Reviewboard];
            string serverName = Server.Names.Type.Reviewboard.ToString();

            logger.Log("Starting authentication");
            logger.Log(" * Name: {0}", serverName);
            logger.Log(" * Url: {0}", serverUrl);

            // Kick off the RB authentation
            Dialogs.SimpleAuthentication authDialog = new Dialogs.SimpleAuthentication(serverName, serverUrl, Reviewboard.Authenticate, logger);
            authDialog.ShowDialog();

            // Load in our credentials object
            Credentials.Credentials rbCredentials = Credentials.Credentials.Create(serverUrl, logger);
            return rbCredentials;

        }

        //
        // Authenticates against the given RB server
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

            // Attempt to authenticate with the server
            string commandOptions = string.Format(@"login --server {0} --username {1} --password {2}", server, username, password);
            string rbtPath = RB_Tools.Shared.Targets.Reviewboard.Path();

            logger.Log("Requesting authentication");
            logger.Log(" * {0} {1}", rbtPath, commandOptions.Replace(password, @"*****"));

            // Run the process
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, rbtPath, commandOptions);
            string errorFlag = "ERROR: ";
            string criticalFlag = "CRITICAL: ";

            // Annoyingly, rbt seems to only post to stderr
            bool succeeded = string.IsNullOrWhiteSpace(output.StdErr) == true;
            if (succeeded == false)
            {
                logger.Log("Querying Standard Error for actual error message");
                succeeded = (output.StdErr.StartsWith(errorFlag) == false && output.StdErr.StartsWith(criticalFlag) == false);
            }

            // How did we do?
            string messageToShow = output.StdErr;
            if (succeeded == true)
            {
                logger.Log("Authentication succeeded");
                messageToShow = @"Successfully authenticated against the Reviewboard server";
            }
            else
            {
                logger.Log("Authentication failed - {0}", messageToShow);
            }

            // Return the result
            return new Result(succeeded, messageToShow.Trim());
        }
    }
}
