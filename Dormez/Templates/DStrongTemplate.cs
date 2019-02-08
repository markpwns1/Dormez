using System;
using Dormez.Types;

namespace Dormez.Templates
{
    public class DStrongTemplate : DTemplate
    {
        Type toInstantiate;

        public DStrongTemplate(Type t)
        {
            toInstantiate = t;
        }

        public override DObject Instantiate(DObject[] inputs, bool init)
        {
            return (DObject)Activator.CreateInstance(toInstantiate, inputs);
        }

        public override string ToString()
        {
            return "strong template " + toInstantiate.Name;
        }
    }
}
