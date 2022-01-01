using CommandLine;
using SchemaInterpreter.Options;
using System;
using System.Linq;
using System.Reflection;

namespace SchemaInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = LoadVerbs();
            Parser.Default.ParseArguments(args, types)
                .WithParsed(Run);
        }

        private static void Run(object obj)
        {
            switch (obj)
            {
                case GenerateVerb t:
                    Console.WriteLine("hello world: " + t.FilePath);
                    break;
            }
        }

        private static Type[] LoadVerbs() => Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}
