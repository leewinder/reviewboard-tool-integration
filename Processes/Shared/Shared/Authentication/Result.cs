using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Authentication
{
    // The results of an authentication request
    public class Result
    {
        public bool     Success { get; private set; }
        public string   Message { get; private set; }

        // Constructor
        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
