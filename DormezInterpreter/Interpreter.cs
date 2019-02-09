using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dormez;
using Dormez.Evaluation;
using Dormez.Types;

namespace DormezInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            bool hasEmbeddedFile = assem.GetManifestResourceNames().Contains("DormezInterpreter.program.dmz");
            List<Token> tokens;

            if (hasEmbeddedFile)
            {
                string embeddedCode;

                using (Stream stream = assem.GetManifestResourceStream("DormezInterpreter.program.dmz"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        embeddedCode = reader.ReadToEnd();
                    }
                }

                tokens = Lexer.ScanString(embeddedCode);
            }
            else if(args.Length > 0)
            {
                string filename = args[0];

                if (!File.Exists(filename))
                {
                    Console.WriteLine("File does not exist: " + filename);
                    Console.ReadKey();
                    return;
                }

                tokens = Lexer.ScanFile(filename);
            }
            else
            {
                Console.WriteLine("Proper usage: DormezInterpreter.exe <filename> <arg1> <arg2> ...");
                Console.ReadKey();
                return;
            }
            
            var interpreter = new Interpreter(tokens);
            var evaluator = new Evaluator(interpreter);

            //try
            {
                var argList = args.ToList();

                if(!hasEmbeddedFile && args.Length > 0)
                    argList.RemoveAt(0);

                var dArgs = new DSet(argList.Select(x => x.ToDString()));
                interpreter.heap.DeclareGlobalVariable("args", dArgs);

                interpreter.Execute();
            }
            //catch (Exception e)
            {
                //Console.WriteLine("Fatal error: " + e.Message);
                //Console.ReadKey();
            }
        }
    }
}
