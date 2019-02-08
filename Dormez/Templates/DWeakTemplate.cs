using Dormez.Evaluation;
using Dormez.Memory;
using Dormez.Types;

namespace Dormez.Templates
{
    public class DWeakTemplate : DTemplate
    {
        public Interpreter i;
        public InterpreterLocation definition;

        public DTemplate super;

        public override DObject Instantiate(DObject[] inputs, bool initialize = true)
        {
            var continueAt = i.GetLocation();

            // functions should not go to the scope they are defined in, but continue in the scope
            // that they were called in, hence i.Goto(definition) is commented out

            //i.Goto(definition);
            i.pointer = definition.pointer;

            DTable instance = (DTable)i.evaluator.tableOp.unaryFunction.Invoke(null);

            if (super != null)
            {
                instance.members.Add("base", new ReadOnlyMember(super.Instantiate(null, false)));
            }

            i.Goto(continueAt);
            
            if (initialize && instance.MemberExists("constructor"))
            {
                instance.GetFunction("constructor").Call(inputs);
            }

            return instance;
        }
    }
}
