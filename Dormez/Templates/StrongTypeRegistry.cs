using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dormez.Memory;
using Dormez.Types;

namespace Dormez.Templates
{
    public static class StrongTypeRegistry
    {
        public static Dictionary<Type, Dictionary<string, MethodInfo>> strongFunctions = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        public static Dictionary<Type, Dictionary<string, StrongProperty>> strongProperties = new Dictionary<Type, Dictionary<string, StrongProperty>>();

        public static void RegisterFunctions(Type type)
        {
            Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

            foreach (var method in type.GetMethods())
            {
                var memberAttrib = method.GetCustomAttribute<MemberAttribute>();

                if (memberAttrib == null)
                {
                    continue;
                }

                methods.Add(memberAttrib.callableName, method);
            }

            strongFunctions.Add(type, methods);
        }

        public static void RegisterProperties(Type type)
        {
            Dictionary<string, StrongProperty> properties = new Dictionary<string, StrongProperty>();
            
            foreach (var property in type.GetProperties())
            {
                var memberAttrib = property.GetCustomAttribute<MemberAttribute>();

                if (memberAttrib == null)
                {
                    continue;
                }

                var accessors = property.GetAccessors();

                MethodInfo getter = null;
                MethodInfo setter = null;
                
                foreach (var accessor in accessors)
                {
                    if (accessor.Name.StartsWith("get_"))
                    {
                        getter = accessor;
                    }
                    else if (accessor.Name.StartsWith("set_"))
                    {
                        setter = accessor;
                    }
                    else
                    {
                        throw new Exception("Accessor is not an accessor?");
                    }
                }
                
                StrongProperty prop = new StrongProperty(null, getter, setter);

                properties.Add(memberAttrib.callableName, prop);
            }

            strongProperties.Add(type, properties);
        }

        public static DObject Instantiate(Type t, DObject[] parameters)
        {
            return (DObject)Activator.CreateInstance(t, parameters);
        }

        public static DObject ToDormezType(object obj)
        {
            if (obj == null)
            {
                return DUndefined.instance;
            }
            else if (obj.GetType() == typeof(void))
            {
                return DVoid.instance;
            }
            else if (obj.GetType() == typeof(float))
            {
                return ((float)obj).ToDNumber();
            }
            else if (obj.GetType() == typeof(double))
            {
                return ((double)obj).ToDNumber();
            }
            else if (obj.GetType() == typeof(int))
            {
                return ((int)obj).ToDNumber();
            }
            else if (obj.GetType() == typeof(bool))
            {
                return ((bool)obj).ToDBool();
            }
            else if (obj.GetType() == typeof(string))
            {
                return obj.ToString().ToDString();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
            {
                return new DSet((IEnumerable<DObject>)obj);
            }
            else
            {
                return DObject.AssertType<DObject>(obj);
            }
        }
    }
}
