using RB_Tools.Shared.Authentication.Credentials;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RB_Tools.Shared.Authentication.Dialogs
{
    partial class SimpleAuthentication : Form
    {
        // Public Delegates
        public delegate Result RequestAuthentication(string server, string user, string password, Logging.Logging logger);

        //
        // Constructor
        //
        public SimpleAuthentication(string serverName, string server, RequestAuthentication requestAuthentication, Logging.Logging logger)
        {
            InitializeComponent();

            // Save our properties
            m_requestAuthentication = requestAuthentication;
            m_logger = logger;

            // Set up our default state
            textBox_Server.Text = server;
            this.Text = serverName + @" Authentication";
            UpdateAuthenticateDialogState(false);

            // We need to populate the initial content
            PopulateExistingCredentials(server);
        }

        // Private properties
        private RequestAuthentication   m_requestAuthentication = null;
        private Logging.Logging         m_logger = null;

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
                m_logger.Log("Starting generation of Simple credentials");
                args.Result = Credentials.Credentials.Create(server, m_logger);
            };

            authThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Set up the default state before we try to load the credentials
                UpdateAuthenticateDialogState(true);

                // Try and load in the credentials
                Credentials.Credentials credentials = null;
                if (args.Error != null)
                {
                    m_logger.Log("Exception thrown when trying to load the existing credentials for {0}\n\n{1}\n", server, args.Error.Message);

                    string message = string.Format("Exception thrown when trying to load the existing credentials for {0}\n\nException: {1}\n\nDescription: {2}", server, args.Error.GetType().Name, args.Error.Message);
                    MessageBox.Show(this, message, @"Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    credentials = args.Result as Credentials.Credentials;
                    m_logger.Log("Credentials generated");
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
            Credentials.Credentials clearedCredentials = Credentials.Credentials.Clear(textBox_Server.Text, m_logger);
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
                m_logger.Log("Requesting authentication for {0}", server);
                Result authResult = m_requestAuthentication(server, user, password, m_logger);

                // How did we do
                if (authResult.Success == true)
                {
                    m_logger.Log("Authentication completed successfully");

                    // Save the values out
                    Credentials.Credentials.Create(server, user, password, m_logger);
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(this, authResult.Message, @"Authentication Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });

                }
                else
                {
                    m_logger.Log("Authentication failed");
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(this, authResult.Message, @"Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    });
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
                    m_logger.Log("Exception thrown when trying to authenticate with {0}\n\n{1}\n", server, args.Error.Message);

                    string message = string.Format("Exception thrown when trying to authenticate with {0}\n\nException: {1}\n\nDescription: {2}", server, args.Error.GetType().Name, args.Error.Message);
                    MessageBox.Show(this, message, @"Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    m_logger.Log("Authentication complete");

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
