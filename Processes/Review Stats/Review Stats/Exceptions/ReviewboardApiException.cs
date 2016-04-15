using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Review_Stats.Exceptions
{
    //
    // Exception thrown when we fail to successfully make a Reviewboard API call
    //
    class ReviewboardApiException: Exception
    {
        public ReviewboardApiException()
        {
        }

        public ReviewboardApiException(string message) : base(message)
        {
        }

        public ReviewboardApiException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
