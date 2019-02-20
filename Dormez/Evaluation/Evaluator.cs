using System;
using System.Collections.Generic;
using System.Linq;
using Dormez.Functions;
using Dormez.Memory;
using Dormez.Templates;
using Dormez.Types;

namespace Dormez.Evaluation
{
    public class Evaluator
    {
        private Interpreter i;

        public List<List<Operation>> precedences = new List<List<Operation>>();

        public Member lastVariable = null;

        private DVoid dvoid = DVoid.instance;

        public Operation tableOp;

        public Evaluator(Interpreter interpreter)
        {
            i = interpreter;
            interpreter.evaluator = this;

            var include = new Operation("include")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var filename = Evaluate<DString>().ToString();

                    if(i.includes.Contains(filename))
                    {
                        throw new InterpreterException(i.CurrentToken, "File has already been included: " + filename);
                    }

                    i.TryEat("semicolon");

                    var tokens = Lexer.ScanFile(filename);
                    tokens.RemoveAll(x => x == "eof");
                    i.tokens.InsertRange(i.pointer, tokens);

                    i.includes.Add(filename);

                    return dvoid;
                }
            };

            var charLiteral = new Operation("char")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    return i.Eat<char>().ToDChar();
                }
            };

            var baseLiteral = new Operation("base")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var super = i.functionOwners.Peek().GetMember("base");

                    lastVariable = super;

                    if(i.CurrentToken == "l bracket")
                    {
                        i.Eat();
                        var p = i.GetParameters();
                        ((DFunction)super.Value.GetMemberValue("constructor")).Call(p);
                    }
                    return lastVariable.Value;
                }
            };

            var thisLiteral = new Operation("this")
            {
                association = Operation.Association.None,
                unaryFunction = (none) => {
                    lastVariable = new ReadOnlyVariable(i.functionOwners.Peek(), i.depth);
                    return lastVariable.Value;
                }
            };

            var structureLiteral = new Operation("structure")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    DTemplate super = null;

                    if(i.CurrentToken == "extending")
                    {
                        i.Eat();
                        var extending = Evaluate();

                        if(extending is DTemplate)
                        {
                            super = (DTemplate)extending;
                        }
                        else
                        {
                            throw new InterpreterException(i.CurrentToken, "Expected a strong template or a weak template");
                        }
                    }

                    var structure = new DWeakTemplate()
                    {
                        i = i,
                        definition = i.GetLocation(),
                        super = super
                    };

                    while (i.CurrentToken != "l curly")
                    {
                        i.Eat();
                    }

                    i.SkipBlock();

                    return structure;
                }
            };

            var funcLiteral = new Operation("function")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.TryEat("of");
                    var location = i.GetLocation();

                    DWeakFunction func = new DWeakFunction()
                    {
                        i = i,
                        location = location
                    };

                    while (i.CurrentToken != "l curly")
                    {
                        i.Eat();
                    }

                    i.SkipBlock();

                    return func;
                }
            };

            var tableLiteral = new Operation("table")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.UnsafeEat("l curly");

                    var table = new DTable();
                    
                    var members = new Dictionary<string, Member>();
                    var properties = new Dictionary<string, WeakProperty>();
                    
                    while (i.CurrentToken != "r curly")
                    {
                        if(i.CurrentToken == "get")
                        {
                            i.Eat();
                            string name = i.GetIdentifier();
                            DWeakFunction getter = (DWeakFunction)funcLiteral.unaryFunction.Invoke(null);
                            getter.owner = table;
                            properties.Add(name, new WeakProperty(getter));
                        }
                        else if (i.CurrentToken == "set")
                        {
                            i.Eat();
                            string name = i.GetIdentifier();

                            if(!properties.ContainsKey(name))
                            {
                                throw i.Exception("Must declare a getter for " + name + " before declaring a setter.");
                            }

                            DWeakFunction setter = (DWeakFunction)funcLiteral.unaryFunction.Invoke(null);
                            setter.owner = table;

                            properties[name].setter = setter;
                        }
                        else if(i.CurrentToken == "var")
                        {
                            i.Eat();
                            string name = i.GetIdentifier();
                            DObject value = new DUndefined();

                            if (i.CurrentToken == "equals")
                            {
                                i.Eat();
                                value = Evaluate();

                                if (value.GetType() == typeof(DWeakFunction))
                                {
                                    ((DWeakFunction)value).owner = table;
                                }
                            }

                            members.Add(name, new Member(value));
                        }
                        else if(i.CurrentToken == "constructor")
                        {
                            i.Eat();
                            DWeakFunction ctor = (DWeakFunction)funcLiteral.unaryFunction.Invoke(null);
                            ctor.owner = table;

                            members.Add("constructor", new ReadOnlyMember(ctor));
                        }
                        else if(i.CurrentToken == "function")
                        {
                            i.Eat();
                            string name = i.GetIdentifier();

                            DWeakFunction ctor = (DWeakFunction)funcLiteral.unaryFunction.Invoke(null);
                            ctor.owner = table;

                            members.Add(name, new Member(ctor));
                        }
                        else
                        {
                            throw i.Exception("Unexpected token: " + i.CurrentToken.Type);
                        }

                        i.TryEat("semicolon");
                    }

                    properties.ToList().ForEach(x => members.Add(x.Key, x.Value));

                    table.SetMembers(members);

                    i.UnsafeEat("r curly");

                    return table;
                }
            };

            tableOp = tableLiteral;

            var returnStatement = new Operation("return")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.returnValue = Evaluate();
                    return dvoid;
                }
            };

            var continueStatement = new Operation("continue")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.shouldContinue = true;
                    return dvoid;
                }
            };

            var breakStatement = new Operation("break")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.shouldBreak = true;
                    return dvoid;
                }
            };

            var forEachLoop = new Operation("for")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.Eat("each");
                    string varName = i.GetIdentifier();
                    i.Eat("in");
                    DSet set = Evaluate<DSet>();

                    var j = i.heap.DeclareLocal(varName);
                    var beginning = i.GetLocation();

                    i.BeginLoop(beginning);

                    foreach (var item in set.items)
                    {
                        if (i.shouldBreak)
                            break;

                        j.Value = item.Value;
                        i.shouldContinue = false;
                        i.ExecuteLoop();
                        i.Goto(beginning);
                    }
                    
                    i.heap.DeleteLocal(varName);
                    i.EndLoop();

                    return dvoid;
                }
            };

            var forLoop = new Operation("from")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    int from = Evaluate<DNumber>().ToInt();
                    i.Eat("to");
                    int to = Evaluate<DNumber>().ToInt();

                    int increment = 1;

                    if (i.CurrentToken == "by")
                    {
                        i.Eat();
                        increment = Evaluate<DNumber>().ToInt();
                    }
                    i.Eat("with");
                    string varName = i.GetIdentifier();
                    var j = i.heap.DeclareLocal(varName);

                    var beginning = i.GetLocation();

                    i.BeginLoop(beginning);
                    
                    for (int k = from; increment > 0 ? k < to : k > to; k += increment)
                    {
                        if (i.shouldBreak)
                            break;

                        j.Value = k.ToDNumber();

                        i.ExecuteLoop();
                        i.Goto(beginning);
                    }
                    
                    i.heap.DeleteLocal(varName);
                    i.EndLoop();

                    return dvoid;
                }
            };

            var untilLoop = new Operation("until")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var beginning = i.GetLocation();
                    i.BeginLoop(beginning);

                    while (!i.GetCondition())
                    {
                        if (i.shouldBreak)
                            break;
                        
                        i.ExecuteLoop();
                        i.Goto(beginning);
                    }
                    
                    i.EndLoop();

                    return dvoid;
                }
            };

            var whileLoop = new Operation("while")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var beginning = i.GetLocation();

                    i.BeginLoop(beginning);

                    while(i.GetCondition())
                    {
                        if (i.shouldBreak)
                            break;
                        
                        i.ExecuteLoop();
                        i.Goto(beginning);
                    }

                    i.EndLoop();

                    return dvoid;
                }
            };

            var ifStatement = new Operation("if")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    bool satisfied = i.GetCondition();

                    if (satisfied)
                    {
                        i.ExecuteBlock();
                    }
                    else
                    {
                        i.SkipBlock();
                    }

                    while(i.CurrentToken == "else" && i.NextToken == "if")
                    {
                        i.Eat();
                        i.Eat();

                        bool meetsCondition = i.GetCondition();

                        if (satisfied || !meetsCondition)
                        {
                            i.SkipBlock();
                        }
                        else
                        {
                            satisfied = true;
                            i.ExecuteBlock();
                        }
                    }

                    if(i.CurrentToken == "else")
                    {
                        i.Eat();
                        if(satisfied)
                        {
                            i.SkipBlock();
                        }
                        else
                        {
                            i.ExecuteBlock();
                        }
                    }

                    return dvoid;
                }
            };

            var openBracket = new Operation("l curly")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    return dvoid;
                }
            };

            var closeBracket = new Operation("r curly")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    i.Eat();
                    return dvoid;
                }
            };

            var preInc = new Operation("increment")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var right = Evaluate();
                    return lastVariable.Value = (DObject.AssertType<DNumber>(lastVariable.Value).ToFloat() + 1).ToDNumber();
                }
            };

            var postInc = new Operation("increment")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    return lastVariable.Value = (DObject.AssertType<DNumber>(lastVariable.Value).ToFloat() + 1).ToDNumber();
                }
            };

            var preDec = new Operation("decrement")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var right = Evaluate();
                    return lastVariable.Value = (DObject.AssertType<DNumber>(lastVariable.Value).ToFloat() - 1).ToDNumber();
                }
            };

            var postDec = new Operation("decrement")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    return lastVariable.Value = (DObject.AssertType<DNumber>(lastVariable.Value).ToFloat() - 1).ToDNumber();
                }
            };

            var semicolon = new Operation("semicolon")
            {
                association = Operation.Association.None,
                unaryFunction = (none) => dvoid
            };

            var declaration = new Operation("var")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    if(i.CurrentToken == "structure")
                    {
                        i.Eat();
                        string name = i.GetIdentifier();
                        DWeakTemplate value = (DWeakTemplate)structureLiteral.unaryFunction(null);
                        i.heap.DeclareLocal(name, value);
                    }
                    else if(i.CurrentToken == "table")
                    {
                        i.Eat();
                        string name = i.GetIdentifier();
                        DTable value = (DTable)tableLiteral.unaryFunction(null);
                        i.heap.DeclareLocal(name, value);
                    }
                    else if(i.CurrentToken == "function")
                    {
                        i.Eat();
                        string name = i.GetIdentifier();
                        DWeakFunction value = (DWeakFunction)funcLiteral.unaryFunction(null);
                        i.heap.DeclareLocal(name, value);
                    }
                    else
                    {
                        string name = i.Eat<string>("identifier");
                        DObject value = DUndefined.instance;

                        if (i.CurrentToken == "equals")
                        {
                            i.Eat();
                            value = Evaluate();
                        }

                        i.heap.DeclareLocal(name, value);
                    }
                    
                    return dvoid;
                }
            };

            var assignment = new Operation("equals")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    var v = lastVariable;
                    return v.Value = Evaluate();
                }
            };

            var identifier = new Operation("identifier")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    string name = i.Eat<string>("identifier");

                    if(!i.heap.Exists(name))
                    {
                        throw new InterpreterException(i.CurrentToken, "Variable does not exist in this scope: " + name);
                    }

                    lastVariable = i.heap.Get(name);
                    
                    if(i.CurrentToken == "equals")
                    {
                        return DUndefined.instance;
                    }

                    return lastVariable.Value;
                }
            };

            var conditional = new Operation("question mark")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    var ifTrue = Evaluate();
                    i.Eat("else");
                    var ifFalse = Evaluate();

                    if (DObject.AssertType<DBool>(left).ToBool())
                    {
                        return ifTrue;
                    }
                    else
                    {
                        return ifFalse;
                    }
                }
            };

            var traverse = new Operation("dot")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    var name = i.GetIdentifier();
                    var type = left.GetType();

                    if (left.HasMember(name))
                    {
                        lastVariable = left.GetMember(name);

                        if (i.CurrentToken == "equals")
                        {
                            return DUndefined.instance;
                        }
                        
                        return lastVariable.Value;
                    }
                    else if(StrongTypeRegistry.strongFunctions.ContainsKey(type))
                    {
                        var methods = StrongTypeRegistry.strongFunctions[type];

                        if(methods.ContainsKey(name))
                        {
                            var method = methods[name];
                            lastVariable = null;
                            return new DStrongFunction(method, left);
                        }
                    }
                    else if (left.HasMember("base"))
                    {
                        i.pointer -= 2; // recurse back to the dot, with base being the new left side
                        return left.GetMemberValue("base");
                    }

                    throw new InterpreterException(i.CurrentToken, "Object (" + type.Name + ") does not contain member: " + name);
                }
            };

            var methodCall = new Operation("l bracket")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) => {
                    var parameters = i.GetParameters();

                    if(left is DFunction)
                    {
                        return ((DFunction)left).Call(parameters);
                    }
                    else if(left is DTemplate)
                    {
                        return ((DTemplate)left).Instantiate(parameters);
                    }
                    else
                    {
                        throw i.Exception("Can only invoke a function or a template");
                    }
                }
            };

            /*var eof = new Operation("eof")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) => DUndefined.instance
            };*/

            var arrayLiteral = new Operation("l square")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    List<DObject> p = new List<DObject>();

                    if (i.CurrentToken == "r square")
                    {
                        i.Eat();
                        return new DSet(new List<DObject>());
                    }

                    bool firstTime = true;

                    do
                    {
                        if (!firstTime)
                            i.Eat("comma");

                        p.Add(Evaluate());

                        firstTime = false;
                    }
                    while (i.CurrentToken == "comma");

                    i.Eat("r square");

                    return new DSet(p);
                }
            };

            var undefinedLiteral = new Operation("undefined")
            {
                association = Operation.Association.None,
                unaryFunction = (none) => DUndefined.instance
            };

            var numberLiteral = new Operation("number")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) => i.Eat<float>().ToDNumber()
            };

            var stringLiteral = new Operation("string")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) => i.Eat<string>().ToDString()
            };

            var boolLiteral = new Operation("bool")
            {
                eatOperator = false,
                association = Operation.Association.None,
                unaryFunction = (none) => i.Eat<bool>().ToDBool()
            };

            var index = new Operation("l square")
            {
                association = Operation.Association.Left,
                unaryFunction = (left) =>
                {
                    lastVariable = left.OpINDEX(Evaluate());
                    i.Eat("r square");
                    return lastVariable.Value;
                }
            };

            var bracket = new Operation("l bracket")
            {
                association = Operation.Association.None,
                unaryFunction = (none) =>
                {
                    var result = Evaluate();
                    i.Eat("r bracket");
                    return result;
                }
            };

            var neg = new Operation("minus")
            {
                association = Operation.Association.Right,
                unaryFunction = (right) => right.OpNEG()
            };

            var pow = new Operation("exponent") {
                binaryFunction = (left, right) => left.OpPOW(right)
            };

            var mul = new Operation("multiply")
            {
                binaryFunction = (left, right) => left.OpMUL(right)
            };

            var div = new Operation("divide")
            {
                binaryFunction = (left, right) => left.OpDIV(right)
            };

            var mod = new Operation("modulus")
            {
                binaryFunction = (left, right) => left.OpMOD(right)
            };

            var add = new Operation("plus")
            {
                binaryFunction = (left, right) => left.OpADD(right)
            };

            var sub = new Operation("minus")
            {
                binaryFunction = (left, right) => left.OpSUB(right)
            };

            var not = new Operation("not")
            {
                association = Operation.Association.Right,
                unaryFunction = (right) => DObject.AssertType<DBool>(right).OpNOT()
            };

            var eq = new Operation("double equal")
            {
                binaryFunction = (left, right) => left.OpEQ(right)
            };

            var neq = new Operation("not equal")
            {
                binaryFunction = (left, right) => left.OpNEQ(right)
            };

            var gr = new Operation("greater than")
            {
                binaryFunction = (left, right) => left.OpGR(right)
            };

            var ls = new Operation("less than")
            {
                binaryFunction = (left, right) => left.OpLS(right)
            };

            var geq = new Operation("greater or equal")
            {
                binaryFunction = (left, right) => left.OpGEQ(right)
            };

            var leq = new Operation("less or equal")
            {
                binaryFunction = (left, right) => left.OpLEQ(right)
            };

            var and = new Operation("and")
            {
                binaryFunction = (left, right) => DObject.AssertType<DBool>(left).OpAND(DObject.AssertType<DBool>(right))
            };

            var or = new Operation("or")
            {
                binaryFunction = (left, right) => {
                    if(left.Equals(DUndefined.instance))
                    {
                        return right;
                    }

                    return DObject.AssertType<DBool>(left).OpOR(DObject.AssertType<DBool>(right));
                }
            };

            //operations.Add(eof);

            void register(Operation op)
            {
                precedences.Last().Add(op);
            }

            void precedence()
            {
                precedences.Add(new List<Operation>());
            }

            precedence();
            register(thisLiteral);
            register(baseLiteral);
            register(bracket);
            register(undefinedLiteral);
            register(arrayLiteral);
            register(numberLiteral);
            register(stringLiteral);
            register(boolLiteral);
            register(tableLiteral);
            register(charLiteral);

            precedence();
            register(identifier);
            register(methodCall);
            register(traverse);
            register(index);

            precedence();
            register(assignment);
            register(preInc);
            register(postInc);

            register(preDec);
            register(postDec);

            precedence();
            register(neg);
            register(not);

            precedence();
            register(pow);

            precedence();
            register(mul);
            register(div);
            register(mod);

            precedence();
            register(add);
            register(sub);

            precedence();
            register(eq);
            register(neq);
            register(gr);
            register(ls);
            register(geq);
            register(leq);

            precedence();
            register(and);

            precedence();
            register(or);

            precedence();
            register(conditional);

            precedence();
            register(semicolon);
            register(declaration);
            register(closeBracket);
            register(openBracket);
            register(ifStatement);
            register(whileLoop);
            register(untilLoop);
            register(forEachLoop);
            register(continueStatement);
            register(breakStatement);
            register(funcLiteral);
            register(returnStatement);
            register(forLoop);
            register(structureLiteral);
            register(include);
        }

        public T Evaluate<T>()
        {
            return DObject.AssertType<T>(Evaluate());
        }

        public DObject Evaluate()
        {
            var result = Evaluate(precedences.Count - 1);

            if (result == null)
            {
                throw new InterpreterException(guiltyToken, "Unexpected token: " + guiltyToken);
            }

            return result;
        }
        
        private Token guiltyToken;

        public DObject Evaluate(int precedence)
        {
            guiltyToken = null;
            if (precedence < 0)
            {
                guiltyToken = i.CurrentToken;
                return null;
            }

            DObject result = Evaluate(precedence - 1);

            List<Operation> operations = new List<Operation>(precedences[precedence]);

            void InvalidateOperation(bool ateOperator, Operation o)
            {
                operations.Remove(o);
                if (ateOperator)
                    i.pointer--;
            }
            
            while(operations.Exists(x => x.operatorToken == i.CurrentToken.Type))
            {
                var operation = operations.Find(x => x.operatorToken == i.CurrentToken.Type);

                if (operation.eatOperator)
                {
                    i.Eat();
                }

                if (operation.IsBinary)
                {
                    result = operation.binaryFunction.Invoke(result, Evaluate(precedence - 1));
                }
                else if (operation.association == Operation.Association.Right)
                {
                    if (result != null || result == DVoid.instance)
                    {
                        InvalidateOperation(operation.eatOperator, operation);
                        continue;
                    }

                    result = operation.unaryFunction.Invoke(Evaluate(precedence - 1));
                }
                else if (operation.association == Operation.Association.Left)
                {
                    if (result == null || result == DVoid.instance)
                    {
                        InvalidateOperation(operation.eatOperator, operation);
                        continue;
                    }

                    result = operation.unaryFunction.Invoke(result);
                }
                else if (operation.association == Operation.Association.None)
                {
                    if (result != null || result == DVoid.instance)
                    {
                        InvalidateOperation(operation.eatOperator, operation);
                        continue;
                    }
                    result = operation.unaryFunction.Invoke(null);
                }
                else
                {
                    throw new Exception("Operator is unary but has an unassigned association");
                }

                operations = new List<Operation>(precedences[precedence]);
            }

            if (result == null)
            {
                return Evaluate(precedence - 1);
            }

            return result;
        }

    }
}
