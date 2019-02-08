using System;

namespace Dormez.StrongFunctions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StrongTemplateAttribute : Attribute
    {
        public string name;

        public StrongTemplateAttribute(string name)
        {
            this.name = name;
        }
    }
}
