using System;

namespace Dormez.Evaluation
{
    public class InterpreterException : Exception
    {
        public InterpreterException(Token token, string message)
            : base(token.Location.ToString() + ": " + message) { }
    }
}
