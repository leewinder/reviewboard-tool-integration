using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Structures
{
    public class Pair<T, U>
    {
        // Properties
        public T First { get; private set; }
        public U Second { get; private set; }

        //
        // Constructor
        //
        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        //
        // Constructor
        //
        public Pair()
        {
        }
    }
}
