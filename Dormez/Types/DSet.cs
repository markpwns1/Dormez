using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Functions;
using Dormez.Memory;

namespace Dormez.Types
{
    public class DSet : DObject
    {
        public List<Variable> items = new List<Variable>();

        public DSet(IEnumerable<DObject> initialized)
        {
            items = initialized.Select(x => new Variable(x)).ToList();
        }

        [Member("at")]
        public DObject Get(DNumber index)
        {
            return items[index.ToInt()].value;
        }

        [Member("set")]
        public DObject Set(DNumber index, DObject value)
        {
            return items[index.ToInt()].Assign(value);
        }

        public override bool Equals(object obj)
        {
            return items.SequenceEqual(AssertType<DSet>(obj).items);
        }

        public override Variable OpINDEX(DObject other)
        {
            return items[AssertType<DNumber>(other).ToInt()];
        }

        public override DObject Clone()
        {
            return new DSet(items.Select(x => x.value.Clone()));
        }

        public override string ToString()
        {
            string s = "[ ";
            for (int i = 0; i < items.Count; i++)
            {
                s += items[i].value.ToString();

                if(i != items.Count - 1)
                {
                    s += ", ";
                }
            }
            s += " ]";
            return s;
        }
    }
}
