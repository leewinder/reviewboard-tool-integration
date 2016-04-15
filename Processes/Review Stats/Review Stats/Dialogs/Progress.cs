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

            // Kick it off and we're done
            Display.SetDisplayProperties(this, label_Progress, progressBar_Progress);
            Generator.Start(this, fileList, debugOptions, () =>
            {
                // Lose our dialog
                this.Close();
            });
        }
    }
}
