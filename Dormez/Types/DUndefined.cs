namespace Dormez.Types
{
    public class DUndefined : DObject
    {
        public static DUndefined instance = new DUndefined();

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(DUndefined))
                return true;

            return false;
        }

        public override string ToString()
        {
            return "undefined";
        }
    }
}
