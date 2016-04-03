using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Authentication.Targets;
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
            // Get our data
            string serverUrl = RB_Tools.Shared.Server.Names.Url[(int)RB_Tools.Shared.Server.Names.Type.Reviewboard];
            string serverName = RB_Tools.Shared.Server.Names.Type.Reviewboard.ToString();

            // Kick off the RB authentation
            SimpleAuthentication authDialog = new SimpleAuthentication(serverName, serverUrl, Reviewboard.Authenticate);
            authDialog.ShowDialog();
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
