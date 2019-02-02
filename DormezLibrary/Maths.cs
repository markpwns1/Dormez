using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Functions;
using Dormez.Memory;

namespace Dormez.Types
{
    [Static("math")]
    public class Maths : DObject
    {
        [Member("sqrt")]
        public DNumber SquareRoot(DNumber n)
        {
            return Math.Sqrt(n.ToFloat()).ToDNumber();
        }
    }
}
