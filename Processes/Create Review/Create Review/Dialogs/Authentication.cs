using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Create_Review
{
    public partial class Authentication : Form
    {
        public Authentication(string workingDirectory)
        {
            InitializeComponent();

            // Save our working directory
            m_workingDirectory = workingDirectory;

            // Fill in the default properties
            UpdateAuthenticateDialogState(true);
            UpdateFieldProperties();
        }

        // Private Properties
        private bool m_passwordDecryptionRequired = false;
        private string m_workingDirectory = string.Empty;

        //
        // Updates the fields with the current values
        //
        private void UpdateFieldProperties()
        {
            // Just update the values
            textBox_ReviewboardServer.Text = Settings.ReviewAuth.Default.Server;
            textBox_ReviewboardUser.Text = Settings.ReviewAuth.Default.User;
            textBox_ReviewboardPassword.Text = Settings.ReviewAuth.Default.Password;

            // If we have a password in the box, then we have something we'll need to decrypt 
            // as it will have come out of the settings
            if (string.IsNullOrWhiteSpace(textBox_ReviewboardPassword.Text) == false)
                m_passwordDecryptionRequired = true;

            // Set the buttons state
            UpdateAuthenticateReviewboardButton();
        }

        //
        // Updates the state of the authentication button
        //
        private void UpdateAuthenticateReviewboardButton()
        {
            // We need a username and password to do this
            bool validUsername = string.IsNullOrWhiteSpace(textBox_ReviewboardUser.Text) == false;
            bool validPassword = string.IsNullOrWhiteSpace(textBox_ReviewboardPassword.Text) == false;

            // Enable if we have valid settings
            button_AuthenticateReviewBoard.Enabled = (validUsername && validPassword);
        }

        //
        // Updates the state of the editable content
        //
        private void UpdateAuthenticateDialogState(bool enabled)
        {
            button_AuthenticateReviewBoard.Enabled = enabled;
            button_AuthenticateReviewBoard.Visible = enabled;

            button_ClearAuthenitcationReviewboard.Enabled = enabled;

            textBox_ReviewboardUser.Enabled = enabled;
            textBox_ReviewboardPassword.Enabled = enabled;

            textBox_ReviewboardServer.Enabled = enabled;

            pictureBox_Authenticating.Enabled = !enabled;
            pictureBox_Authenticating.Visible = !enabled;
        }

        //
        // Clears any settings data to it's default values
        //
        private void button_ClearAuthenitcationReviewboard_Click(object sender, EventArgs e)
        {
            // Reset the settings
            Settings.ReviewAuth.Default.Reset();
            Settings.ReviewAuth.Default.Save();
            Settings.ReviewAuth.Default.Reload();

            // Update the display
            UpdateFieldProperties();
        }

        //
        // Starts the authenticate the reviewboard settings
        //
        private void button_AuthenticateReviewBoard_Click(object sender, EventArgs e)
        {
            // Turn off the buttons and lock the settings
            UpdateAuthenticateDialogState(false);

            // Build up the background work
            BackgroundWorker authThread = new BackgroundWorker();

            // Called when we need to trigger the authentication
            authThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Kick it off
                Reviewboard.ConnectionProperties authRequest = args.Argument as Reviewboard.ConnectionProperties;
                Reviewboard.AuthenticationResult result = Reviewboard.Authenticate(authRequest.Server, authRequest.User, authRequest.Password, authRequest.DecryptPassword);

                // Save the result
                args.Result = result;
            };
            // Called when the thread is complete
            authThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    string body = string.Format("Exception thrown when trying to authenticate with the Reviewboard server\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);
                    Notification.Show(this, @"Unable to authenticate", body, Notification.FormIcon.Cross);
                }
                else
                {
                    // Show the result
                    Reviewboard.AuthenticationResult authResult = args.Result as Reviewboard.AuthenticationResult;
                    if (authResult.Success == true)
                    {
                        Notification.Show(this, @"Authentication Complete", authResult.Message, Notification.FormIcon.Tick);

                        // Save the values that we used
                        Settings.ReviewAuth.Default.Server = textBox_ReviewboardServer.Text;

                        Settings.ReviewAuth.Default.User = textBox_ReviewboardUser.Text;
                        Settings.ReviewAuth.Default.Password = authResult.Password;

                        Settings.ReviewAuth.Default.Authenticated = true;

                        // Save the settings out
                        Settings.ReviewAuth.Default.Save();

                        // Lose this dialog
                        this.Close();
                    }
                    else
                    {
                        Notification.Show(this, @"Authentication Failed", authResult.Message, Notification.FormIcon.Cross);
                    }
                }

                // Set the button state back
                UpdateAuthenticateDialogState(true);
            };

            // Kick off the request
            Reviewboard.ConnectionProperties authProperties = new Reviewboard.ConnectionProperties(m_workingDirectory, textBox_ReviewboardServer.Text, textBox_ReviewboardUser.Text, textBox_ReviewboardPassword.Text, m_passwordDecryptionRequired);
            authThread.RunWorkerAsync(authProperties);
        }

        private void textBox_ReviewboardUser_TextChanged(object sender, EventArgs e)
        {
            // Set the buttons state
            UpdateAuthenticateReviewboardButton();
        }

        private void textBox_ReviewboardPassword_TextChanged(object sender, EventArgs e)
        {
            // Set the buttons state
            UpdateAuthenticateReviewboardButton();

            // As we've manually changed the text, it no longer needs to be decrypted
            m_passwordDecryptionRequired = false;
        }
    }
}
