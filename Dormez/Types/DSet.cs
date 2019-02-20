using System.Collections.Generic;
using System.Linq;
using Dormez.Functions;
using Dormez.Memory;
using Dormez.Templates;

namespace Dormez.Types
{
    public class DSet : DObject
    {
        public List<Member> items = new List<Member>();

        public DSet(IEnumerable<DObject> initialized)
        {
            items = initialized.Select(x => new Member(x)).ToList();
        }

        [Member("at")]
        public DObject Get(DNumber index)
        {
            return items[index.ToInt()].Value;
        }

        [Member("set")]
        public DObject Set(DNumber index, DObject value)
        {
            return items[index.ToInt()].Value = value;
        }

        [Member("add")]
        public void Add(DObject value)
        {
            items.Add(new Member(value));
        }

        [Member("remove")]
        public void Remove(DObject value)
        {
            Member m = null;
            foreach (var item in items)
            {
                if (item.Value.Equals(value))
                {
                    m = item;
                    break;
                }
            }

            if(m != null)
                items.Remove(m);
        }

        [Member("removeAt")]
        public void Remove(DNumber index) => items.RemoveAt(index.ToInt());

        [Member("length")]
        public int Length => items.Count;

        [Member("forEach")]
        public void ForEach(DFunction func)
        {
            for (int i = 0; i < items.Count; i++)
            {
                func.Call(new DObject[] { i.ToDNumber(), items[i].Value });
            }
        }

        [Member("map")]
        public DSet Map(DFunction func)
        {
            DSet newSet = new DSet(new DObject[0]);

            foreach (var item in items)
            {
                newSet.Add(func.Call(new DObject[] { item.Value }));
            }

            return newSet;
        }

        public override bool Equals(object obj)
        {
            return items.SequenceEqual(AssertType<DSet>(obj).items);
        }

        public override Member OpINDEX(DObject other)
        {
            return items[AssertType<DNumber>(other).ToInt()];
        }

        public override DObject Clone()
        {
            return new DSet(items.Select(x => x.Value.Clone()));
        }
        
        public override string ToString()
        {
            string s = "[ ";
            for (int i = 0; i < items.Count; i++)
            {
                s += items[i].Value.ToString();

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
