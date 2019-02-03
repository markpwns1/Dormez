using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.StrongFunctions;

namespace Dormez.Types
{
    [StrongTemplate("Stack")]
    public class DStack : DObject
    {
        public override string ToString()
        {
            return "Stack";
        }
    }
}
