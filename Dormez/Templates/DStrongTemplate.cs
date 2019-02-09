using System;
using Dormez.Types;

namespace Dormez.Templates
{
    public class DStrongTemplate : DTemplate
    {
        Type template;

        public DStrongTemplate(Type templ)
        {
            template = templ;
        }

        public override DObject Instantiate(DObject[] inputs, bool init)
        {
            return StrongTypeRegistry.Instantiate(template, inputs);
        }

        public override string ToString()
        {
            return "(strong template) " + template.Name;
        }
    }
}
