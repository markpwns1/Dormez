using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Dormez.Evaluation;
using Dormez.Functions;
using Dormez.Memory;

/*

NOTES:

    You don't need to override OpNEQ, OpGEQ, or OpLEQ

*/

namespace Dormez.Types
{
    public class DObject
    {

        public static Dictionary<Type, List<StrongFunction>> publicMethods = new Dictionary<Type, List<StrongFunction>>();

        public Dictionary<string, Variable> members = new Dictionary<string, Variable>();

        public DObject()
        {
            Type type = GetType();

            if (!publicMethods.ContainsKey(type))
            {
                List<StrongFunction> methods = new List<StrongFunction>();

                foreach (var method in type.GetMethods())
                {
                    var memberAttrib = method.GetCustomAttribute<Member>();

                    if (memberAttrib == null)
                    {
                        continue;
                    }

                    var registered = new StrongFunction()
                    {
                        callableName = memberAttrib.callableName,
                        method = method
                    };

                    methods.Add(registered);
                }

                foreach(var property in type.GetProperties())
                {
                    var memberAttrib = property.GetCustomAttribute<Member>();

                    if(memberAttrib == null)
                    {
                        continue;
                    }

                    var accessors = property.GetAccessors();

                    foreach (var accessor in accessors)
                    {
                        StrongFunction registered = new StrongFunction();

                        registered.method = accessor;

                        if(accessor.Name.StartsWith("get_"))
                        {
                            registered.callableName = "get" + memberAttrib.callableName;
                        }
                        else if(accessor.Name.StartsWith("set_"))
                        {
                            registered.callableName = "set" + memberAttrib.callableName;
                        }
                        else
                        {
                            throw new Exception("Accessor is not an accessor?");
                        }

                        methods.Add(registered);
                    }
                }

                publicMethods.Add(type, methods);
            }

            
        }

        protected Exception OpException(Type leftSide)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return new Exception("Operator " + sf.GetMethod().Name + " cannot be used with type " + leftSide.Name);
        }

        protected Exception OpException(Type leftSide, Type rightSide)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return new Exception("Operator " + sf.GetMethod().Name + " cannot be used with between the types " + leftSide.Name + " and " + rightSide.Name);
        }

        public static T AssertType<T>(object obj)
        {
            if(obj is T)
            {
                return (T)obj;
            }
            else
            {
                if(obj == null)
                {
                    throw new Exception("Expected type " + typeof(T).Name + ", got null");
                }
                else
                {
                    throw new Exception("Expected type " + typeof(T).Name + ", got type " + obj.GetType().Name);
                }
            }
        }

        // EQUALITY OPERATORS
        public virtual DBool OpEQ(DObject other)
        {
            return Equals(other).ToDBool();
        }
        
        public virtual DBool OpGR(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }
        
        public virtual DBool OpLS(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        // SPINOFF EQUALITY OPERATORS (NO OVERRIDING REQUIRED)
        public virtual DBool OpGEQ(DObject other)
        {
            return OpGR(other).OpOR(OpEQ(other));
        }

        public virtual DBool OpNEQ(DObject other)
        {
            return OpEQ(other).OpNOT();
        }

        public virtual DBool OpLEQ(DObject other)
        {
            return OpLS(other).OpOR(OpEQ(other));
        }

        // VALUE OPERATORS
        public virtual DObject OpADD(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        public virtual DObject OpSUB(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        public virtual DObject OpMUL(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }
        
        public virtual DObject OpNEG()
        {
            throw OpException(GetType());
        }

        public virtual DObject OpDIV(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        public virtual DObject OpMOD(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        public virtual DObject OpPOW(DObject other)
        {
            throw OpException(GetType(), other.GetType());
        }

        public virtual Variable OpINDEX(DObject other)
        {
            throw new Exception("Type " + GetType() + " cannot be indexed");
        }

        [Member("getType")]
        public DString DGetType()
        {
            return GetType().Name.ToDString();
        }

        [Member("toString")]
        public DString DToString()
        {
            return ToString().ToDString();
        }

        [Member("clone")]
        public virtual DObject Clone()
        {
            throw new Exception("Cannot clone type: " + GetType().Name);
        }
    }

    public static class Conversions
    {
        public static DBool ToDBool(this bool v)
        {
            return new DBool(v);
        }

        public static DNumber ToDNumber(this int v)
        {
            return new DNumber(v);
        }

        public static DNumber ToDNumber(this float v)
        {
            return new DNumber(v);
        }

        public static DNumber ToDNumber(this double v)
        {
            return new DNumber((float)v);
        }

        public static DString ToDString(this string v)
        {
            return new DString(v);
        }

        public static DChar ToDChar(this char v)
        {
            return new DChar(v);
        }
    }
}
