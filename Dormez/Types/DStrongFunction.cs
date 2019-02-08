using System.Collections;
using System.Collections.Generic;
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
            if (methodInfo.method.ReturnType == typeof(void))
            {
                methodInfo.method.Invoke(parent, parameters);
                return DVoid.instance;
            }
            else if(methodInfo.method.ReturnType == typeof(float))
            {
                return ((float)methodInfo.method.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.method.ReturnType == typeof(double))
            {
                return ((double)methodInfo.method.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.method.ReturnType == typeof(double))
            {
                return ((int)methodInfo.method.Invoke(parent, parameters)).ToDNumber();
            }
            else if(methodInfo.method.ReturnType == typeof(bool))
            {
                return ((bool)methodInfo.method.Invoke(parent, parameters)).ToDBool();
            }
            else if(methodInfo.method.ReturnType == typeof(string))
            {
                return methodInfo.method.Invoke(parent, parameters).ToString().ToDString();
            }
            else if(typeof(IEnumerable).IsAssignableFrom(methodInfo.method.ReturnType))
            {
                return new DSet((IEnumerable<DObject>)methodInfo.method.Invoke(parent, parameters));
            }
            else
            {
                var result = methodInfo.method.Invoke(parent, parameters);

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
            return "strong function " + methodInfo.method.DeclaringType.Name + "." + methodInfo.callableName;
        }
    }
}
