namespace Dormez.Types
{
    public class DVoid : DObject
    {
        public static DVoid instance = new DVoid();

        public override bool Equals(object obj)
        {
            throw OpException(GetType(), obj.GetType());
        }

        public override string ToString()
        {
            return "void";
        }
    }
}
