using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Functions;
using Dormez.Memory;
using Dormez.StrongFunctions;

namespace Dormez.Types
{
    [StrongTemplate("Map")]
    public class DMap : DObject
    {
        public Dictionary<DObject, Variable> pairs = new Dictionary<DObject, Variable>();

        [Member("constructor")]
        public void Constructor() { }

        [Member("getKeys")]
        public DSet GetKeys()
        {
            return new DSet(pairs.Keys);
        }

        [Member("length")]
        public DNumber Length()
        {
            return pairs.Count.ToDNumber();
        }

        [Member("remove")]
        public void Remove(DObject key)
        {
            pairs.Remove(key);
        }

        [Member("add")]
        public void Add(DObject key, DObject value)
        {
            pairs.Add(key, new Variable(value));
        }

        public override Variable OpINDEX(DObject other)
        {
            var key = pairs.Keys.ToList().Find(x => x.Equals(other));

            if (key == null)
            {
                throw new InterpreterException(Interpreter.current.CurrentToken, "No key exists: " + other.ToString());
            }

            return pairs[key];
        }

        public override string ToString()
        {
            string output = "[\n";

            var keys = pairs.Keys.ToList();
            var values = pairs.Values.ToList();

            for (int i = 0; i < pairs.Count; i++)
            {
                output += " " + keys[i].ToString() + ": " + values[i].value.ToString() + "\n";
            }

            output += "]";

            return output;
        }
    }
}
