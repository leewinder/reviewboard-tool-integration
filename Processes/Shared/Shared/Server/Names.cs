using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Server
{
    public sealed class Names 
    {
        // Server Types
        public enum Type
        {
            Reviewboard,
            Jira,
        }

        // Server Urls
        public static readonly string[] Url =
        {
            @"http://localhost/reviewboard",
            @"http://localhost:8080",
        };
    }
}
