using Dormez.Evaluation;
using Dormez.Memory;

namespace Dormez.Types
{
    public class DWeakTemplate : DObject
    {
        public Interpreter i;
        public InterpreterLocation definition;

        public DWeakTemplate weakSuper;
        public DStrongTemplate strongSuper;
        
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

            if(weakSuper != null)
            {
                instance.members.Add("base", new Member(weakSuper.ShallowInstantiate()));
            }
            else if(strongSuper != null)
            {
                instance.members.Add("base", new Member(strongSuper.Instantiate(new DObject[0])));
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
