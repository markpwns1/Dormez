using System;

namespace Dormez.Memory
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StaticAttribute : Attribute
    {
        public string name;

        public StaticAttribute(string n)
        {
            name = n;
        }
    }
}
