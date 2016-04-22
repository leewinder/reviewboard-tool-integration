using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Logging.Implementations
{
    class LoggerNull : Logging
    {
        //
        // Constructor
        //
        public LoggerNull()
        {
            // Set the write callback
            WriteText = WriteNothing;
        }

        //
        // Null writing object
        //
        private void WriteNothing(string logMessage)
        {
        }
    }
}
