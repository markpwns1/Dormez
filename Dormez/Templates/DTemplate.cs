using Dormez.Types;

namespace Dormez.Templates
{
    public abstract class DTemplate : DObject
    {
        public abstract DObject Instantiate(DObject[] parameters, bool initialize = true);
    }
}
