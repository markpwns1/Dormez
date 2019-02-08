using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Types;

namespace Dormez.Functions
{
    public abstract class DFunction : DObject
    {
        public abstract DObject Call(DObject[] parameters);
    }
}
