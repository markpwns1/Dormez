using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;

namespace Dormez.Types
{
    public class DWeakTemplate : DObject
    {
        public Interpreter i;
        public InterpreterLocation definition;

        public DWeakTemplate super;
        
        // Instantiates without calling constructor
        public DTable ShallowInstantiate()
        {
            return Instantiate(null, false);
        }

        public DTable Instantiate(DObject[] inputs, bool callConstructor = true)
        {
            var continueAt = i.GetLocation();

            i.Goto(definition);

            DTable instance = (DTable)i.evaluator.tableOp.unaryFunction.Invoke(null);

            if(super != null)
            {
                instance.members.Add("base", new Memory.Variable(super.ShallowInstantiate()));
            }

            i.Goto(continueAt);

            if (callConstructor && instance.MemberExists("constructor"))
            {
                instance.GetFunction("constructor").Call(inputs);
            }

            return instance;
        }
    }
}
