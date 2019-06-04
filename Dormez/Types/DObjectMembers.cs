using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Functions;
using Dormez.Memory;
using Dormez.Templates;

namespace Dormez.Types
{
    public partial class DObject
    {
        public Dictionary<string, Member> members = new Dictionary<string, Member>();

        public DObject()
        {
            Type type = GetType();

            if (!StrongTypeRegistry.strongFunctions.ContainsKey(type))
            {
                StrongTypeRegistry.RegisterFunctions(type);
                StrongTypeRegistry.RegisterProperties(type);
            }

            var t = GetType();
            if (StrongTypeRegistry.strongProperties.ContainsKey(t))
            {
                StrongTypeRegistry.strongProperties[t].ToList().ForEach(x => {
                    x.Value.owner = this;
                    members.Add(x.Key, x.Value);
                });
            }
        }

        public bool HasMember(string name)
        {
            return members.ContainsKey(name);
        }

        public Member GetMember(string name)
        {
            if (!HasMember(name))
                throw Interpreter.current.Exception("Object of type " + GetType() + " does not contain member: " + name);

            return members[name];
        }
        
        public DObject GetMemberValue(string name)
        {
            return GetMember(name).Value;
        }

        public DObject CallFunction(string name, DObject[] parameters)
        {
            return AssertType<DFunction>(GetMemberValue(name)).Call(parameters);
        }

        public DObject CLRCallFunction(string name, params DObject[] parameters)
        {
            return AssertType<DFunction>(GetMemberValue(name)).Call(parameters);
        }

        public void AddMember(string name, Member m)
        {
            members.Add(name, m);
        }

        public void DeleteMember(string name)
        {
            members.Remove(name);
        }
    }
}
