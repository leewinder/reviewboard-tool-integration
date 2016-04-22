using RB_Tools.Shared.Logging;
using Review_Stats.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Review_Stats.Dialogs
{
    public partial class Progress : Form
    {
        public Progress(string fileList, string debugOptions)
        {
            InitializeComponent();

            // Enable logging
            Logging logger = Logging.Create("Generate Review Stats", Logging.Type.File, Logging.Threading.MultiThread);

            // Kick it off and we're done
            Display.SetDisplayProperties(this, label_Progress, progressBar_Progress, logger);
            Generator.Start(this, fileList, logger, debugOptions, () =>
            {
                // Lose our dialog
                logger.Log("Closing dialog - work done");
                this.Close();
            });
        }
    }
}
