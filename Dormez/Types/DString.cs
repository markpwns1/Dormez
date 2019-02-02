using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Functions;

namespace Dormez.Types
{
    public class DString : DObject
    {
        private string value;

        public DString(object v)
        {
            value = v.ToString();
        }

        public override bool Equals(object obj)
        {
            return value == obj.ToString();
        }

        public override DObject OpADD(DObject other)
        {
            return new DString(value + other.ToString());
        }

        [Member("length")]
        public DNumber Length()
        {
            return value.Length.ToDNumber();
        }

        [Member("toNumber")]
        public DNumber ToDNumber()
        {
            return int.Parse(value).ToDNumber();
        }

        public override DObject Clone()
        {
            return value.ToDString();
        }

        public override string ToString()
        {
            return value;
        }
    }
}
