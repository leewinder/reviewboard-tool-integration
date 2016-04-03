using System;
using System.Windows.Forms;

using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Authentication.Targets;

namespace Authentication.Dialogs
{
    public partial class CredentialOptions : Form
    {
        public CredentialOptions()
        {
            InitializeComponent();
        }

        //
        // Authorise reviewboard
        //
        private void button_ReviewboardAuthentication_Click(object sender, EventArgs e)
        {
            // Just authenticate
            Reviewboard.Authenticate();
        }

        //
        // Clear everything
        //
        private void button_ClearAllAuthentication_Click(object sender, EventArgs e)
        {
            string[] serverList = RB_Tools.Shared.Server.Names.Url;
            foreach (string thisServer in serverList)
                Credentials.Clear(thisServer);
        }
    }
}
