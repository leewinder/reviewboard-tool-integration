using System;

namespace RB_Tools.Shared.Authentication.Targets
{
    // Authentication for a Reviewboard Server
    public class Reviewboard
    {
        //
        // Starts authenticating against the Reviewboard server
        //
        public static Credentials.Credentials Authenticate()
        {
            // Get our data
            string serverUrl = Server.Names.Url[(int)Server.Names.Type.Reviewboard];
            string serverName = Server.Names.Type.Reviewboard.ToString();

            // Kick off the RB authentation
            Dialogs.SimpleAuthentication authDialog = new Dialogs.SimpleAuthentication(serverName, serverUrl, Reviewboard.Authenticate);
            authDialog.ShowDialog();

            // Load in our credentials object
            Credentials.Credentials rbCredentials = Credentials.Credentials.Create(serverUrl);
            return rbCredentials;

        }
        //
        // Authenticates against the given RB server
        //
        private static Result Authenticate(string server, string username, string password)
        {
            // Check we have the properties we need
            bool validServer = string.IsNullOrWhiteSpace(server) == false;
            bool validUser = string.IsNullOrWhiteSpace(username) == false;
            bool validPassword = string.IsNullOrWhiteSpace(password) == false;

            // If it's not valid, throw and we're done
            if (validServer == false || validUser == false || validPassword == false)
                throw new ArgumentException("Invalid server, username or password requested");

            // Attempt to authenticate with the server
            string commandOptions = string.Format(@"login --server {0} --username {1} --password {2}", server, username, password);
            string rbtPath = RB_Tools.Shared.Targets.Reviewboard.Path();

            // Run the process
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, rbtPath, commandOptions);
            string errorFlag = "ERROR: ";
            string criticalFlag = "CRITICAL: ";

            // Annoyingly, rbt seems to only post to stderr
            bool succeeded = string.IsNullOrWhiteSpace(output.StdErr) == true;
            if (succeeded == false)
                succeeded = (output.StdErr.StartsWith(errorFlag) == false && output.StdErr.StartsWith(criticalFlag) == false);

            // How did we do?
            string messageToShow = output.StdErr;
            if (succeeded == true)
            {
                messageToShow = @"Successfully authenticated against the Reviewboard server";
            }

            // Return the result
            return new Result(succeeded, messageToShow.Trim());
        }
    }
}
