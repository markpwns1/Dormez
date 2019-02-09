using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Templates;
using Dormez.Types;

namespace Dormez.Memory
{
    public class StrongProperty : Member
    {
        public MethodInfo getter;
        public MethodInfo setter;
        public DObject owner;

        public StrongProperty(DObject owner, MethodInfo getter, MethodInfo setter) : base(null)
        {
            this.getter = getter;
            this.setter = setter;
            this.owner = owner;
        }

        protected override DObject GetValue()
        {
            return StrongTypeRegistry.ToDormezType(getter.Invoke(owner, null));
        }

        protected override DObject SetValue(DObject value)
        {
            if(setter == null)
            {
                throw Interpreter.current.Exception("Attempt to write to read-only variable");
            }

            setter.Invoke(owner, new object[] { value });
            return value;
        }
    }
}
