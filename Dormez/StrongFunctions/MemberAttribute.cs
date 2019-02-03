using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.Functions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MemberAttribute : Attribute
    {
        public string callableName;

        public MemberAttribute(string name)
        {
            callableName = name;
        }
    }
}
