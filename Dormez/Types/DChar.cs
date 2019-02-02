using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.Types
{
    public class DChar : DObject
    {
        public char value;

        public DChar(char v)
        {
            value = v;
        }
        
        public override string ToString()
        {
            return value.ToString();
        }
    }
}
