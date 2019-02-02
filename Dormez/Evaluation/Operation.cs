using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Types;

namespace Dormez.Evaluation
{
    public class Operation
    {
        public enum Association
        {
            Left,
            Right,
            None
        }

        public string operatorToken;

        public Func<DObject, DObject, DObject> binaryFunction;
        // (left, right) => { }

        public bool eatOperator = true;

        public Association association; // only applies to unary functions
        public Func<DObject, DObject> unaryFunction;
        // If left associative
        // (left) => { }

        // If right associative
        // (right) => { }


        public Operation(string tok)
        {
            operatorToken = tok;
        }

        public bool IsBinary
        {
            get { return unaryFunction == null; }
        }
    }
}
