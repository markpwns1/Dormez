using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Functions;
using Dormez.Memory;

namespace Dormez.Types
{
    [Static("console")]
    public class DConsole : DObject
    {
        [Member("Title")]
        public DString Title
        {
            get { return Console.Title.ToDString(); }
            set { Console.Title = value.ToString(); }
        }

        [Member("print")]
        public void Print(DObject obj)
        {
            Console.WriteLine(obj.ToString());
        }

        [Member("write")]
        public void Write(DObject obj)
        {
            Console.Write(obj.ToString());
        }

        [Member("readLine")]
        public DString ReadLine()
        {
            return Console.ReadLine().ToDString();
        }

        [Member("readKey")]
        public DChar ReadKey()
        {
            return Console.ReadKey().KeyChar.ToDChar();
        }

        public override string ToString()
        {
            return "console";
        }
    }
}
