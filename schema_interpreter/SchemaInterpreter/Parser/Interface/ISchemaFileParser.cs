using SchemaInterpreter.Parser.Definition;
using System.IO;
using System.Threading.Tasks;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Defines a method to parse schema files.
    /// </summary>
    public interface ISchemaFileParser
    {
        /// <summary>
        /// The version of the schema parser.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Parses a schema file.
        /// </summary>
        /// <param name="reader">The stream reader used to read the schema.</param>
        public Task<SchemaFile> ParseFile(StreamReader reader);
    }
}