using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Types;

namespace Dormez.Memory
{
    public class ReadOnlyVariable : Variable
    {
        public ReadOnlyVariable(DObject value, int depth) : base(value, depth) { }

        protected override DObject SetValue(DObject value)
        {
            throw Interpreter.current.Exception("Attempt to write to read-only variable");
        }
    }
}
