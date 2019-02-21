using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Templates;

namespace Dormez.Types
{
    [StrongTemplate("Exception")]
    public class DException : DObject
    {
        [Member("message")]
        public DString message { get; set; }

        public DException(DString msg)
        {
            message = msg;
        }

        public override string ToString()
        {
            return message.ToString();
        }
    }
}
