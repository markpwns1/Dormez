using System;

namespace Dormez.Evaluation
{
    public class InterpreterException : Exception
    {
        public CodeLocation location;
        public string message;

        public InterpreterException(Token token, string message)
            : base(token.Location.ToString() + ": " + message)
        {
            this.location = token.Location;
            this.message = message;
        }

        public InterpreterException(CodeLocation location, string message) 
            : base(location.ToString() + ": " + message)
        {
            this.location = location;
            this.message = message;
        }

        public InterpreterException(string message) : base(message) { }

        public override string ToString()
        {
            return location.ToString() + ": " + message;
        }
    }
}
