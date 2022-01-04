using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchemaInterpreter.Parser.V1
{
    /// <summary>
    /// Responsible of reading and interpreting schema files.
    /// </summary>
    public static class SchemaParser
    {
        private static readonly ISchemaFileParser mParser = new SchemaFileParser();

        /// <summary>
        /// Parses files.
        /// </summary>
        /// <param name="path">The path containing the directory or file.</param>
        public static async Task<IEnumerable<SchemaFile>> ParseFiles(string path)
        {
            bool? isDir = IsDirectory(path);
            if (isDir == null)
                throw new FileLoadException("Invalid path exception.", path);
            else if (isDir == true)
            {
                List<SchemaFile> schemaFiles = new();
                foreach(var file in Directory.EnumerateFiles(path, "*.mpack", SearchOption.TopDirectoryOnly))
                {
                    using StreamReader reader = OpenFile(file);
                    var parsed = await mParser.ParseFile(reader);
                    schemaFiles.Add(parsed);
                }

                return schemaFiles;
            }
            else
            {
                using StreamReader reader = OpenFile(path);
                var parsed = await mParser.ParseFile(reader);
                return parsed.AsEnumerable();
            }
        }

        private static StreamReader OpenFile(string file)
        {
            return new StreamReader(File.OpenRead(file));
        }

        private static bool? IsDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;
            else if (File.Exists(path))
                return false;
            else return null;
        }
    }
}