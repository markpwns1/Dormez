using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dormez.Templates;
using Dormez.Types;

namespace Dormez.Functions
{
    public class DStrongFunction : DFunction
    {
        public DObject parent;
        public MethodInfo methodInfo;

        public DStrongFunction(MethodInfo m, DObject parent)
        {
            methodInfo = m;
            this.parent = parent;
        }

        public override DObject Call(DObject[] parameters)
        {
            var result = methodInfo.Invoke(parent, parameters);
            return StrongTypeRegistry.ToDormezType(result);
        }

        public override string ToString()
        {
            return "strong function";
        }
    }
}
