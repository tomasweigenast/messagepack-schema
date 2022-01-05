using CommandLine;
using SchemaInterpreter.Plugin.Encoder;

namespace SchemaInterpreter.Options
{
    [Verb("generate", HelpText = "Generates source code for a schema file.")]
    public class GenerateVerb
    {
        [Option('i', "input", Required = true, HelpText = "The path to the file or directory containing files to generate.")]
        public string FilePath { get; set; }

        [Option('e', "encoding", Required = false, HelpText = "Sets the encoding used to send the generated schema to the plugin.", Default = PluginEncoding.MessagePack)]
        public PluginEncoding Encoding { get; set; }

        [Option('p', "plugin", Required = true, HelpText = "The path to the plugin executable used to generate source code files.")]
        public string PluginPath { get; set; }

        [Option("verbose", Default = false, HelpText = "Log debug messages.", Required = false)]
        public bool Verbose { get; set; }
    }
}