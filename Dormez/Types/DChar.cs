namespace Dormez.Types
{
    public class DChar : DObject
    {
        public char value;

        public DChar(char v)
        {
            value = v;
        }

        public override bool Equals(object obj)
        {
            return obj.ToString() == value.ToString();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
