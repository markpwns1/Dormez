using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.StrongFunctions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Extension : Attribute
    {
        public string callableName;

        public Extension(string name)
        {
            callableName = name;
        }
    }
}
