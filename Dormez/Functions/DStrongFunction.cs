using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(parent, parameters);
                return DVoid.instance;
            }
            else if(methodInfo.ReturnType == typeof(float))
            {
                return ((float)methodInfo.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.ReturnType == typeof(double))
            {
                return ((double)methodInfo.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.ReturnType == typeof(double))
            {
                return ((int)methodInfo.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.ReturnType == typeof(bool))
            {
                return ((bool)methodInfo.Invoke(parent, parameters)).ToDBool();
            }
            else if(methodInfo.ReturnType == typeof(string))
            {
                return methodInfo.Invoke(parent, parameters).ToString().ToDString();
            }
            else if(typeof(IEnumerable).IsAssignableFrom(methodInfo.ReturnType))
            {
                return new DSet((IEnumerable<DObject>)methodInfo.Invoke(parent, parameters));
            }
            else
            {
                var result = methodInfo.Invoke(parent, parameters);

                if(result == null)
                {
                    return DUndefined.instance;
                }
                else
                {
                    return (DObject)result;
                }
            }
        }

        public override string ToString()
        {
            return "strong function";
        }
    }
}
