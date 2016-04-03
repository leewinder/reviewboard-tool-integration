using RB_Tools.Shared.Authentication.Credentials;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RB_Tools.Shared.Authentication.Dialogs
{
    partial class SimpleAuthentication : Form
    {
        // Public Delegates
        public delegate Result RequestAuthentication(string server, string user, string password);

        //
        // Constructor
        //
        public SimpleAuthentication(string serverName, string server, RequestAuthentication requestAuthentication)
        {
            InitializeComponent();

            // Save our properties
            m_requestAuthentication = requestAuthentication;

            // Set up our default state
            textBox_Server.Text = server;
            this.Text = serverName + @" Authentication";
            UpdateAuthenticateDialogState(false);

            // We need to populate the initial content
            PopulateExistingCredentials(server);
        }

        // Private properties
        private RequestAuthentication   m_requestAuthentication = null;

        //
        // Updates the state of the authentication button
        //
        private void UpdateAuthenticateButton()
        {
            // We need a username and password to do this
            bool validUsername = string.IsNullOrWhiteSpace(textBox_User.Text) == false;
            bool validPassword = string.IsNullOrWhiteSpace(textBox_Password.Text) == false;

            // Enable if we have valid settings
            button_Authenticate.Enabled = (validUsername && validPassword);
        }

        //
        // Updates the state of the editable content
        //
        private void UpdateAuthenticateDialogState(bool enabled)
        {
            button_Authenticate.Enabled = enabled;
            button_Authenticate.Visible = enabled;

            button_ClearAuthenitcation.Enabled = enabled;

            textBox_User.Enabled = enabled;
            textBox_Password.Enabled = enabled;

            textBox_Server.Enabled = enabled;

            pictureBox_Authenticating.Enabled = !enabled;
            pictureBox_Authenticating.Visible = !enabled;
        }

        //
        // Updates the fields with the current values
        //
        private void UpdateFieldProperties(Credentials.Credentials credentials)
        {
            // We use simple credentials
            Simple simpleCredentials = credentials as Simple;

            // Just update the values
            textBox_User.Text = (credentials == null ? string.Empty : simpleCredentials.User);
            textBox_Password.Text = (credentials == null ? string.Empty : simpleCredentials.Password);

            // Set the buttons state
            UpdateAuthenticateButton();
        }

        //
        // Populates any existing credentials
        //
        private void PopulateExistingCredentials(string server)
        {
            // Load our credentials, since this can take a while do it on another thread
            BackgroundWorker authThread = new BackgroundWorker();

            // Called when we need to trigger the authentication
            authThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                args.Result = Credentials.Credentials.Create(server);
            };

            authThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Set up the default state before we try to load the credentials
                UpdateAuthenticateDialogState(true);

                // Try and load in the credentials
                Credentials.Credentials credentials = null;
                if (args.Error != null)
                {
                    string message = string.Format("Exception thrown when trying to load the existing credentials for {0}\n\nException: {1}\n\nDescription: {2}", server, args.Error.GetType().Name, args.Error.Message);
                    MessageBox.Show(this, message, @"Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    credentials = args.Result as Credentials.Credentials;
                }

                // Load our credentials
                UpdateFieldProperties(credentials);
            };
            // Kick off the request
            authThread.RunWorkerAsync();
        }

        //
        // User has updated their user name
        //
        private void textBox_User_TextChanged(object sender, EventArgs e)
        {
            // Set the buttons state
            UpdateAuthenticateButton();
        }

        //
        // User has updated their user name
        //
        private void textBox_Password_TextChanged(object sender, EventArgs e)
        {
            // Set the buttons state
            UpdateAuthenticateButton();
        }

        //
        // User wants to clear the credentials
        //
        private void button_ClearAuthenitcation_Click(object sender, EventArgs e)
        {
            Credentials.Credentials clearedCredentials = Credentials.Credentials.Clear(textBox_Server.Text);
            UpdateFieldProperties(clearedCredentials);
        }

        //
        // User wants to authenticate
        //
        private void button_Authenticate_Click(object sender, EventArgs e)
        {
            // Turn off the buttons and lock the settings
            UpdateAuthenticateDialogState(false);

            // Pull out our properties
            string server = textBox_Server.Text;
            string user = textBox_User.Text;
            string password = textBox_Password.Text;

            // Build up the background work
            BackgroundWorker authThread = new BackgroundWorker();

            // Called when we need to trigger the authentication
            authThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Request authentication and save the results
                Result authResult = m_requestAuthentication(server, user, password);
                if (authResult.Success == true)
                {
                    // Save the values out
                    Credentials.Credentials.Create(server, user, password);
                    MessageBox.Show(this, authResult.Message, @"Authentication Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, authResult.Message, @"Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // Save the results
                args.Result = authResult;
            };

            // Called when the thread is complete
            authThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    string message = string.Format("Exception thrown when trying to authenticate with {0}\n\nException: {1}\n\nDescription: {2}", server, args.Error.GetType().Name, args.Error.Message);
                    MessageBox.Show(this, message, @"Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Result results = args.Result as Result;
                    if (results.Success == true)
                        this.Close();
                }

                // Set the button state back
                UpdateAuthenticateDialogState(true);
            };

            // Kick off the request
            authThread.RunWorkerAsync();
        }
    }
}
