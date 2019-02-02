using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Functions;

namespace Dormez.Types
{
    public class DStrongFunction : DObject
    {
        public DObject parent;
        public StrongFunction methodInfo;

        public DStrongFunction(StrongFunction m, DObject parent)
        {
            methodInfo = m;
            this.parent = parent;
        }

        public DObject Call(object[] parameters)
        {
            if(methodInfo.method.ReturnType == typeof(void))
            {
                methodInfo.method.Invoke(parent, parameters);
                return DVoid.instance;
            }
            else
            {
                return (DObject)methodInfo.method.Invoke(parent, parameters);
            }
        }

        public override string ToString()
        {
            return "strong function " + methodInfo.method.DeclaringType.Name + "." + methodInfo.callableName;
        }
    }
}
