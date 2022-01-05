using CommandLine;
using SchemaInterpreter.Helpers;
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
                .WithParsedAsync(RunAsync)
                .Wait();
        }

        private static async Task RunAsync(object obj)
        {
            try
            {
                switch (obj)
                {
                    case GenerateVerb t:
                        if (t.Verbose)
                            Logger.SetMinimumLogLevel(LogLevel.Debug);

                        var compiledFiles = await SchemaCompiler.CompileFiles(t.FilePath);
                        Logger.Info($"Compiled {compiledFiles.Count()} file(s).");

                        await PluginManager.RunPluginAsync(t.PluginPath, t.OutputFolder, compiledFiles, t.Encoding);
                        Logger.Info("Execution finished successfully.");
                        break;
                }
            }
            catch(Exception ex)
            {
                Logger.Error("An error occurred.");
                Logger.Error(ex.ToString());
            }
        }

        private static Type[] LoadVerbs() => Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}
