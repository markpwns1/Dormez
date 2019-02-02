﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dormez.Memory;
using Dormez.Types;

namespace Dormez.Evaluation
{
    public class Interpreter
    {
        public static Interpreter current;

        public Heap heap;

        public Stack<InterpreterLocation> loopLocations = new Stack<InterpreterLocation>();
        public Stack<Variable> callers = new Stack<Variable>();

        public List<string> includes = new List<string>();

        public List<Token> tokens = new List<Token>();
        public Evaluator evaluator;

        public DObject returnValue = null;

        public int pointer = 0;
        public int depth = 0;

        public bool shouldBreak = false;
        public bool shouldContinue = false;

        static IEnumerable<Type> GetClasses(Assembly asm, string nameSpace)
        {
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace);
        }

        static IEnumerable<Type> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace);
        }

        public Interpreter(List<Token> tokens)
        {
            //current = this;
            this.tokens = tokens;
            this.heap = new Heap(this);

            var classes = GetClasses("Dormez.Types").ToList();

            if (Directory.Exists("lib"))
            {
                foreach (var file in Directory.GetFiles("lib"))
                {
                    var asm = Assembly.LoadFrom(file);
                    classes.AddRange(GetClasses(asm, "Dormez.Types"));
                }
            }

            foreach (var c in classes)
            {
                var staticAttrib = c.GetCustomAttribute<Static>();
                if(staticAttrib != null)
                {
                    heap.DeclareGlobalVariable(staticAttrib.name, (DObject)Activator.CreateInstance(c));
                }
            }
        }

        public Token NextToken
        {
            get { return tokens[pointer + 1]; }
        }

        public Token CurrentToken
        {
            get { return tokens[pointer]; }
        }

        public Token PreviousToken
        {
            get { return tokens[pointer - 1]; }
        }

        public Token LookAhead(int amount)
        {
            return tokens[pointer + amount];
        }

        private void ChangeDepth(int desiredDepth)
        {
            depth = desiredDepth;
        }

        public string GetIdentifier()
        {
            return Eat<string>("identifier");
        }

        public T Eat<T>(string type)
        {
            if (CurrentToken == type)
            {
                return Eat<T>();
            }

            throw new InterpreterException(CurrentToken, "Expected " + type + " but got " + CurrentToken + " at token #" + pointer);
        }

        public object Eat(string type)
        {
            return Eat<object>(type);
        }

        public T Eat<T>()
        {
            if(CurrentToken == "l curly")
            {
                depth++;
                //Console.WriteLine("->");
            }
            else if(CurrentToken == "r curly")
            {
                depth--;
                heap.DeleteUnscopedVariables();
                //Console.WriteLine("<-");
            }

            if(depth < 0)
            {
                throw new InterpreterException(CurrentToken, "Depth is less than zero!");
            }

            pointer++;

            return (T) PreviousToken.Value;
        }

        public void UnsafeEat(string type)
        {
            if (CurrentToken == type)
            {
                pointer++;
                return;
            }

            throw new InterpreterException(CurrentToken, "Expected " + type + " but got " + CurrentToken + " at token #" + pointer);
        }

        public void TryEat(string type)
        {
            if (CurrentToken == type)
            {
                Eat();
            }
        }

        public object Eat()
        {
            return Eat<object>();
        }

        public bool GetCondition()
        {
            return DObject.AssertType<DBool>(evaluator.Evaluate()).ToBool();
        }

        public InterpreterLocation GetLocation()
        {
            return new InterpreterLocation()
            {
                pointer = pointer,
                depth = depth
            };
        }
        
        public DObject[] GetParameters()
        {
            List<DObject> p = new List<DObject>();
            
            if(CurrentToken == "r bracket")
            {
                Eat();
                return new DObject[0];
            }

            while (true)
            {
                p.Add(evaluator.Evaluate());

                if(CurrentToken == "r bracket")
                {
                    Eat("r bracket");
                    break;
                }
                else
                {
                    Eat("comma");
                }
            }

            return p.ToArray();
        }

        public void Goto(InterpreterLocation location)
        {
            pointer = location.pointer;
            depth = location.depth;
            heap.DeleteUnscopedVariables();
        }
        
        public void Execute()
        {
            while(CurrentToken != "eof")
            {
                evaluator.Evaluate();
            }
        }

        public void ExecuteLoop()
        {
            shouldContinue = false;
            int originalDepth = depth;
            Eat("l curly");
            while (depth != originalDepth)
            {
                evaluator.Evaluate();

                if (shouldContinue || shouldBreak)
                {
                    AbortLoop();
                    break;
                }
            }
        }

        public DObject ExecuteFunction(InterpreterLocation functionLocation)
        {
            int originalDepth = depth;
            Eat("l curly");

            returnValue = null;

            while (depth != originalDepth)
            {
                if(returnValue != null || shouldBreak)
                {
                    while (depth > originalDepth + 1)
                    {
                        Eat();
                    }

                    break;
                }

                evaluator.Evaluate();
            }
            
            var v = returnValue == null ? DVoid.instance : returnValue;
            
            returnValue = null;

            return v;
        }

        public void ExecuteBlock()
        {
            int originalDepth = depth;
            Eat("l curly");
            while (depth != originalDepth)
            {
                evaluator.Evaluate();
            }
        }

        public void SkipBlock()
        {
            int originalDepth = depth;
            Eat("l curly");
            while (depth != originalDepth)
            {
                Eat();
            }
        }

        public void AbortLoop()
        {
            var loc = loopLocations.Peek();
            while (depth > loc.depth)
            {
                Eat();
            }
        }

        public void BeginLoop(InterpreterLocation beginning)
        {
            loopLocations.Push(beginning);
        }

        public void EndLoop()
        {
            SkipBlock();
            loopLocations.Pop();
            shouldBreak = false;
        }
    }
}