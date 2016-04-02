using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Authentication.Credentials
{
    public class Simple : Credentials
    {
        public string   User { get; private set; }
        public string   Password { get; private set; }
    }
}
