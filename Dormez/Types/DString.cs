using System.Linq;
using Dormez.Templates;

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
        
        [Member("replace")]
        public string Replace(DString from, DString to) => value.Replace(from.ToString(), to.ToString());

        [Member("contains")]
        public bool Contains(DString v) => value.Contains(v.ToString());
        
        [Member("endsWith")]
        public bool EndsWith(DString v) => value.EndsWith(v.ToString());
        
        [Member("startsWith")]
        public bool StartsWith(DString v) => value.StartsWith(v.ToString());

        [Member("length")]
        public int Length() => value.Length;

        [Member("toNumber")]
        public int ToDNumber() => int.Parse(value);

        [Member("getChars")]
        public DSet GetChars()
        {
            return new DSet(value.ToCharArray().Select(x => x.ToDChar()));
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
