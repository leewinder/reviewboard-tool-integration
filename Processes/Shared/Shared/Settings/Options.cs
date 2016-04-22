using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Settings
{
    public class Options
    {
        public bool EnableLogging { get; set; }

        // Constructor
        public Options()
        {
            EnableLogging = false;
        }
    }
}
