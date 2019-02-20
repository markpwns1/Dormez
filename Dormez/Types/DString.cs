using System.Linq;
using Dormez.Memory;
using Dormez.Templates;

namespace Dormez.Types
{
    [StrongTemplate("string")]
    public class DString : DObject
    {
        public string value;

        public DString(DSet charArray)
        {
            value = new string(charArray.items.Select(x => DObject.AssertType<DChar>(x.Value).value).ToArray());
        }

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
        public int Length => value.Length;

        [Member("firstIndexOf")]
        public int IndexOf(DString v) => value.IndexOf(v.ToString());

        [Member("lastIndexOf")]
        public int LastIndexOf(DString v) => value.LastIndexOf(v.ToString());

        [Member("toNumber")]
        public int ToDNumber() => int.Parse(value);

        [Member("characters")]
        public DSet GetChars => new DSet(value.ToCharArray().Select(x => x.ToDChar()));

        [Member("split")]
        public DSet Split(DChar delimiter) => new DSet(value.Split(delimiter.value).Select(x => x.ToDString()));

        [Member("remove")]
        public string Remove(DNumber index) => value.Remove(index.ToInt());

        [Member("substring")]
        public string Substring(DNumber index) => value.Substring(index.ToInt());

        [Member("trim")]
        public string Trim() => value.Trim();

        [Member("toLowerCase")]
        public string ToLower() => value.ToLowerInvariant();

        [Member("toUpperCase")]
        public string ToUpper() => value.ToUpperInvariant();

        [Member("insert")]
        public string Insert(DNumber index, DString with) => value.Insert(index.ToInt(), value.ToString());

        [Member("set")]
        public string Set(DNumber index, DChar with)
        {
            var chars = value.ToCharArray();
            chars[index.ToInt()] = with.value;
            return new string(chars);
        }

        public override Member OpINDEX(DObject other)
        {
            int index = AssertType<DNumber>(other).ToInt();
            return new StringChar(this, index);
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
