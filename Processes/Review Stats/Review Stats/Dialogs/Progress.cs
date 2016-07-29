using RB_Tools.Shared.Logging;
using Review_Stats.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Review_Stats.Dialogs
{
    public partial class Progress : Form
    {
        public Progress(string fileList, bool injectPaths, Logging logger)
        {
            InitializeComponent();

            // Set up
            m_fileList = fileList;
            m_injectPaths = injectPaths;

            m_logger = logger;
        }

        // Private Properties
        private readonly string     m_fileList = null;
        private readonly bool       m_injectPaths = false;

        private readonly Logging    m_logger = null;
        
        //
        // We want to generate the report
        //
        private void button_GenerateReport_Click(object sender, EventArgs e)
        {
            // We need a report name
            string expectedTextRegEx = @"^[a-zA-Z0-9][A-Za-z0-9_. -]*$";
            if (string.IsNullOrWhiteSpace(textBox_ReportName.Text) == true || Regex.IsMatch(textBox_ReportName.Text, expectedTextRegEx) == false)
            {
                m_logger.Log(@"No file name provided when generating report");
                MessageBox.Show("You need to provide a valid Report Name before generating the stats.\n\nValid characters are 'a-z', 'A-Z', '0-9', _, ., [space] and -.  The name must start with either a-z or 0-9", @"Stats Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Format the report name
            string reportName = textBox_ReportName.Text.Trim();

            // Kick it off and we're done
            Display.SetDisplayProperties(this, label_Progress, progressBar_Progress, button_GenerateReport, m_logger);
            Generator.Start(this, m_fileList, reportName, m_logger, m_injectPaths, () =>
            {
                // Lose our dialog
                m_logger.Log("Closing dialog - work done");
                this.Close();
            });
        }
    }
}
