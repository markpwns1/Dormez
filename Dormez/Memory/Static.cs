using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.Memory
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Static : Attribute
    {
        public string name;

        public Static(string n)
        {
            name = n;
        }
    }
}
