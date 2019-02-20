using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Types;

namespace Dormez.Memory
{
    public class StringChar : Member
    {
        public DString container;

        public char character;
        public int index;

        public StringChar(DString co, int i) : base(null)
        {
            container = co;
            index = i;

            character = container.ToString()[index];
        }

        protected override DObject GetValue()
        {
            return character.ToDChar();
        }

        protected override DObject SetValue(DObject value)
        {
            var chars = container.ToString().ToCharArray();
            chars[index] = DObject.AssertType<DChar>(value).value;
            container.value = new string(chars);
            return container;
        }
    }
}
