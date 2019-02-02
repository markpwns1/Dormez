using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez.Evaluation
{
    public class InterpreterException : Exception
    {
        public InterpreterException(Token token, string message)
            : base(token.Location.ToString() + ": " + message) { }
    }
}
