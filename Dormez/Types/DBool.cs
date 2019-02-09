namespace Dormez.Types
{
    public class DBool : DObject
    {
        private bool value;

        public DBool(bool val)
        {
            value = val;
        }

        public override bool Equals(object obj)
        {
            return (obj.GetType() == typeof(DBool)) && ((obj as DBool).value == value);
        }

        public DBool OpNOT()
        {
            return new DBool(!value);
        }

        public DBool OpAND(DBool other)
        {
            return new DBool(value && other.value);
        }

        public DBool OpOR(DBool other)
        {
            return new DBool(value || other.value);
        }

        public override string ToString()
        {
            return value.ToString().ToLower();
        }

        public override DObject Clone()
        {
            return value.ToDBool();
        }

        public bool ToBool()
        {
            return value;
        }
    }
}
