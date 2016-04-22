using System;
using System.Windows.Forms;

using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Authentication.Targets;
using RB_Tools.Shared.Logging;
using RB_Tools.Shared.Settings;

namespace Settings.Dialogs
{
    public partial class SettingOptions : Form
    {
        public SettingOptions(Logging logger)
        {
            InitializeComponent();

            // Load our settings
            m_settings = RB_Tools.Shared.Settings.Settings.Load();
            UpdateDialogOptions(m_settings);

            // Save our logger
            m_logger = logger;
        }

        // Properties
        private readonly Logging    m_logger;
        private Options             m_settings;

        //
        // Updates the dialog with the given options
        //
        private void UpdateDialogOptions(Options savedSettings)
        {
            // Update the options
            checkBox_Logging.Checked = savedSettings.EnableLogging;
        }

        //
        // Authorise reviewboard
        //
        private void button_ReviewboardAuthentication_Click(object sender, EventArgs e)
        {
            // Just authenticate
            Reviewboard.Authenticate(m_logger);
        }

        //
        // Clear everything
        //
        private void button_ClearAllAuthentication_Click(object sender, EventArgs e)
        {
            string[] serverList = RB_Tools.Shared.Server.Names.Url;
            foreach (string thisServer in serverList)
                Credentials.Clear(thisServer, m_logger);
        }

        //
        // Close the dialog
        //
        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //
        // Enabled logging changed
        //
        private void checkBox_Logging_CheckedChanged(object sender, EventArgs e)
        {
            m_settings.EnableLogging = checkBox_Logging.Checked;
            RB_Tools.Shared.Settings.Settings.Save(m_settings);
        }
    }
}
