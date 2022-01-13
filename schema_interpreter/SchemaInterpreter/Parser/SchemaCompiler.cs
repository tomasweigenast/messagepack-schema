using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SchemaInterpreter.Parser.V1
{
    /// <summary>
    /// Responsible of reading and interpreting schema files.
    /// </summary>
    public static class SchemaCompiler
    {
        private static readonly ISchemaFileParser mParser = new SchemaFileParserV1();

        /// <summary>
        /// Compiles a list of files.
        /// </summary>
        /// <param name="path">The path containing the directory or file.</param>
        public static async Task<IEnumerable<SchemaPackage>> CompileFiles(string path)
        {
            // Create a new ParserContext
            ParserContext.CreateContext();

            // Compile files
            await InternalCompile(path);

            // Verify all imported packages
            ParserContext.Current.VerifyImports();

            // Generate default values
            ParserContext.Current.GenerateDefaultValues();

            // Verify all values
            ParserContext.Current.VerifyAllTypes();

            // Verify enums
            ParserContext.Current.VerifyEnums();

            // Return compiled files
            return ParserContext.Current.GetCompiledAndClear();
        }

        private static async Task InternalCompile(string path)
        {
            bool? isDir = CommonHelpers.IsDirectory(path);
            if (isDir == null)
                throw new FileLoadException("Invalid path.", path);
            else if (isDir == true)
            {
                foreach (var file in Directory.EnumerateFiles(path, $"*{Keywords.FileExtension}", SearchOption.TopDirectoryOnly))
                {
                    string packageName = Path.GetFileNameWithoutExtension(file);

                    using StreamReader reader = OpenFile(file);
                    await mParser.ParseFile(reader, packageName);
                }

            }
            else
            {
                if (Path.GetExtension(path) != Keywords.FileExtension)
                    throw new FileLoadException("Invalid file extension.", path);

                string packageName = Path.GetFileNameWithoutExtension(path);

                using StreamReader reader = OpenFile(path);
                await mParser.ParseFile(reader, packageName);
            }
        }

        private static StreamReader OpenFile(string file)
        {
            return new StreamReader(File.OpenRead(file));
        }
    }
}