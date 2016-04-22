using System;
using System.Windows.Forms;

using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Authentication.Targets;
using RB_Tools.Shared.Logging;

namespace Authentication.Dialogs
{
    public partial class CredentialOptions : Form
    {
        public CredentialOptions(Logging logger)
        {
            InitializeComponent();

            // Save our logger
            m_logger = logger;
        }

        // Properties
        private readonly Logging m_logger;

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
    }
}
