using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;

namespace Dormez.Types
{
    public class DWeakFunction : DObject
    {
        public DTable owner = null;
        public Interpreter i;
        public InterpreterLocation location;

        public DObject Call(DObject[] inputs)
        {
            var continueAt = i.GetLocation();

            i.Goto(location);

            List<string> parameters = new List<string>();
            
            bool first = true;

            while (i.CurrentToken != "l curly")
            {
                if(!first)
                {
                    i.Eat("comma");
                }

                first = false;

                parameters.Add(i.GetIdentifier());
            }

            if(parameters.Count != inputs.Length)
            {
                throw new InterpreterException(i.CurrentToken, "Parameter count mismatch: expected " + parameters.Count + " arguments but got " + inputs.Length);
            }

            for (int j = 0; j < parameters.Count; j++)
            {
                i.heap.DeclareLocalVariable(parameters[j], inputs[j]);
            }

            if(owner != null)
            {
                i.callers.Push(new Memory.Variable(owner));
                //i.DeclareLocalVariable("this", owner);
            }

            var value = i.ExecuteFunction(location);

            foreach (var p in parameters)
            {
                i.heap.Delete(p);
            }

            if(owner != null)
            {
                i.callers.Pop();
                //i.variables.Remove("this");
            }

            i.Goto(continueAt);

            return value;
        }

        public override string ToString()
        {
            return "weak function";
        }

        public DObject CLRCall(params DObject[] args)
        {
            return Call(args);
        }
    }

    
}
