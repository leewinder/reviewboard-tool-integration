using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Review_Stats.Statistics
{
    class Display
    {
        //
        // Set our properties
        //
        public static void SetDisplayProperties(Label progressLabel)
        {
            // Just save it
            m_progressLabel = progressLabel;
        }

        // Properties
        private static Label m_progressLabel;
    }
}
