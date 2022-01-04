using CommandLine;

namespace SchemaInterpreter.Options
{
    [Verb("generate", HelpText = "Generates source code for a schema file.")]
    public class GenerateVerb
    {
        [Option('i', "input", Required = true, HelpText = "The path to the file or directory containing files to generate.")]
        public string FilePath { get; set; }
    }
}