using RB_Tools.Shared.Authentication;
using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Authentication.Dialogs
{
    public partial class SimpleAuthentication : Form
    {
        // Public Delegates
        public delegate Result RequestAuthentication(string server, string user, string password);

        //
        // Constructor
        //
        public SimpleAuthentication(string server, RequestAuthentication requestAuthentication)
        {
            InitializeComponent();

            // Save our properties
            m_requestAuthentication = requestAuthentication;

            // Set up our default state
            textBox_Server.Text = server;
            UpdateAuthenticateDialogState(true);

            // Load our credentials
            Credentials existingCredentials = Credentials.Create(server);
            UpdateFieldProperties(existingCredentials);
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
        private void UpdateFieldProperties(Credentials credentials)
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
            Credentials clearedCredentials = Credentials.Clear(textBox_Server.Text);
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
                    Credentials.Create(server, user, password);
                    MessageBox.Show(authResult.Message, @"Authentication Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(authResult.Message, @"Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    MessageBox.Show(message, @"Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
