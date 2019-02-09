using System;

namespace Dormez
{
    class LexerException : Exception
    {
        public LexerException(CodeLocation location, string message) : base(location.ToString() + " - " + message) { }
    }
}
