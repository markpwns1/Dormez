using System.Collections.Generic;
using Dormez.Evaluation;
using Dormez.Types;

namespace Dormez.Memory
{
    using VarStack = PseudoStack<Variable>;

    public class Heap
    {
        public Interpreter interpreter;
        public Dictionary<string, VarStack> variables = new Dictionary<string, VarStack>();

        public Heap(Interpreter i)
        {
            this.interpreter = i;
        }

        public Variable DeclareGlobalVariable(string name, DObject val)
        {
            if(variables.ContainsKey(name))
            {
                throw new InterpreterException(interpreter.CurrentToken, "Global variable already exists: " + name);
            }

            var stack = new VarStack();
            var v = new Variable(val, -1);
            stack.Push(v);
            variables.Add(name, stack);
            return v;
        }
        
        public Variable DeclareLocalVariable(string name)
        {
            return DeclareLocalVariable(name, null);
        }

        public Variable DeclareLocalVariable(string name, DObject val)
        {
            val = val == null ? DUndefined.instance : val;

            var v = new Variable(val, interpreter.depth);

            if(!variables.ContainsKey(name))
            {
                var stack = new VarStack();
                stack.Push(v);
                variables.Add(name, stack);
            }

            variables[name].Push(v);

            return v;
        }

        public Variable Get(string name)
        {
            return variables[name].Peek();
        }

        public DObject GetValue(string name)
        {
            return variables[name].Peek().Value;
        }

        public void Delete(string name)
        {
            variables[name].Pop();

            if(variables[name].Count <= 0)
            {
                variables.Remove(name);
            }
        }

        public void DeleteUnscopedVariables()
        {
            var keys = new List<string>(variables.Keys);
            foreach (var name in keys)
            {
                var toDelete = new List<Variable>();

                foreach (var variable in variables[name])
                {
                    if (variable.Depth > interpreter.depth)
                    {
                        toDelete.Add(variable);
                    }
                }

                foreach (var item in toDelete)
                {
                    variables[name].Remove(item);
                }

                if (variables[name].Count <= 0)
                {
                    variables.Remove(name);
                }
            }
        }

        public bool Exists(string name)
        {
            return variables.ContainsKey(name);
        }
    }
}
