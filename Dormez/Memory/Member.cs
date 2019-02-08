using Dormez.Types;

namespace Dormez.Memory
{
    public class Member
    {
        protected DObject _value;

        public DObject Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public Member(DObject value)
        {
            _value = value;
        }

        protected virtual DObject GetValue()
        {
            return _value;
        }

        protected virtual DObject SetValue(DObject value)
        {
            return _value = value;
        }
    }
}
