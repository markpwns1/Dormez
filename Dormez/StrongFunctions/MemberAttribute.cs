using System;

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
