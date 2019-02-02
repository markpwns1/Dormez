using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dormez
{
    class LexerException : Exception
    {
        public LexerException(CodeLocation location, string message) : base(location.ToString() + " - " + message) { }
    }
}
