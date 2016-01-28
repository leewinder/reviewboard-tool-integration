using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace About
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            // Set the version information
            string[] versionNumbers = GetVersionAndBuildNumbers();
            if (versionNumbers != null)
            {
                label_VersionNumber.Text = versionNumbers[0];
                label_BuildNumber.Text = versionNumbers[1];
            }
        }

        //
        // Gets the current version and build numbers
        //
        private string[] GetVersionAndBuildNumbers()
        {
            // Get where we are
            string executaionLocation = Assembly.GetExecutingAssembly().Location;
            string execuationPath = Path.GetDirectoryName(executaionLocation);

            // Get the version content
            string[] versionNumbers = null;
            try
            {
                versionNumbers = File.ReadAllLines(execuationPath + "\\..\\version");
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to find the version information file\n\n" + e.Message, "Unable to show About");
                return null;
            }

            // Check we have two entries with actual content
            if (versionNumbers.Length != 2)
            {
                MessageBox.Show("Version information file doesn't contain the expected content count", "Unable to show About");
                return null;
            }

            // Check we have two entries with actual content
            if (string.IsNullOrWhiteSpace(versionNumbers[0]) == true || string.IsNullOrWhiteSpace(versionNumbers[1]) == true)
            {
                MessageBox.Show("Version information file doesn't contain the version information expected", "Unable to show About");
                return null;
            }

            return versionNumbers;
        }

        private void linkLabel_ProjectWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_ProjectWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/leewinder/reviewboard-tool-integration");
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
