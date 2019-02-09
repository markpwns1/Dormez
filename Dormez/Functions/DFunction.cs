using Dormez.Types;

namespace Dormez.Functions
{
    public abstract class DFunction : DObject
    {
        public abstract DObject Call(DObject[] parameters);
    }
}
