using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Functions;
using Dormez.Types;

namespace Dormez.Memory
{
    public class WeakProperty : Member
    {
        public DWeakFunction getter;
        public DWeakFunction setter;

        public WeakProperty(DWeakFunction getter) : base(null)
        {
            this.getter = getter;
        }

        protected override DObject GetValue()
        {
            var lastVariable = Interpreter.current.evaluator.lastVariable;
            var result = getter.Call(new DObject[0]);
            Interpreter.current.evaluator.lastVariable = lastVariable;
            return result;
        }

        protected override DObject SetValue(DObject value)
        {
            if(setter == null)
            {
                throw Interpreter.current.Exception("Attempt to write to read-only variable");
            }

            return setter.Call(new DObject[] { value });
        }
    }
}
