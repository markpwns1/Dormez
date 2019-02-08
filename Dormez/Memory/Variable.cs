using Dormez.Evaluation;
using Dormez.Types;

namespace Dormez.Memory
{
    public class Variable : Member
    {
        public int Depth { get; }
        
        public Variable(DObject value, int depth) : base(value)
        {
            this.Depth = depth;
        }

        protected override DObject GetValue()
        {
            if(Interpreter.current.depth < Depth)
            {
                Interpreter.current.ThrowException("Attempted to access an out of scope variable");
            }

            return _value;
        }
    }
}
