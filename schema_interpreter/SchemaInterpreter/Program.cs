using CommandLine;
using SchemaInterpreter.Options;
using SchemaInterpreter.Parser.V1;
using SchemaInterpreter.Plugin;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SchemaInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = LoadVerbs();
            var parser = new CommandLine.Parser((with) =>
            {
                with.CaseInsensitiveEnumValues = false;
            });

            parser.ParseArguments(args, types)
                .WithParsed((obj) => Run(obj).Wait());
        }

        private static async Task Run(object obj)
        {
            switch (obj)
            {
                case GenerateVerb t:
                    var compiledFiles = await SchemaCompiler.CompileFiles(t.FilePath);
                    await PluginManager.RunPluginAsync(t.PluginPath, compiledFiles, t.Encoding);
                    break;
            }
        }

        private static Type[] LoadVerbs() => Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}
