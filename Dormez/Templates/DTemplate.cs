using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Types;

namespace Dormez.Templates
{
    public abstract class DTemplate : DObject
    {
        public abstract DObject Instantiate(DObject[] parameters, bool initialize = true);
    }
}
