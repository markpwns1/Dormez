using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Types;

namespace Dormez.Memory
{
    public class ReadOnlyMember : Member
    {
        public ReadOnlyMember(DObject value) : base(value) { }

        protected override DObject SetValue(DObject value)
        {
            throw Interpreter.current.Exception("Attempt to write to read-only variable");
        }
    }
}
