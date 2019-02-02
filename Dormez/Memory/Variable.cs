using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dormez.Types;
using System.Threading.Tasks;

namespace Dormez.Memory
{
    public class Variable
    {
        public DObject value;
        public int depth;

        public Variable(DObject value, int depth = -1)
        {
            this.depth = depth;
            this.value = value;
        }

        public DObject Assign(DObject value)
        {
            return this.value = value;
        }
    }
}
