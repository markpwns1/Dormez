﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.Types
{
    public class DStrongTemplate : DObject
    {
        Type toInstantiate;

        public DStrongTemplate(Type t)
        {
            toInstantiate = t;
        }

        public DObject Instantiate(DObject[] inputs)
        {
            return (DObject)Activator.CreateInstance(toInstantiate, inputs);
        }

        public override string ToString()
        {
            return "strong template " + toInstantiate.Name;
        }
    }
}