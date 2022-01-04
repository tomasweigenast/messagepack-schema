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
        private static readonly ISchemaFileParser mParser = new SchemaFileParser();

        /// <summary>
        /// Compiles a list of files.
        /// </summary>
        /// <param name="path">The path containing the directory or file.</param>
        public static async Task<IEnumerable<SchemaFile>> CompileFiles(string path)
        {
            // Create a new ParserContext
            ParserContext.Create();

            // Compile files
            await InternalCompile(path);

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
                List<SchemaFile> schemaFiles = new();
                foreach (var file in Directory.EnumerateFiles(path, $"*{Keywords.FileExtension}", SearchOption.TopDirectoryOnly))
                {
                    string packageName = Path.GetFileNameWithoutExtension(file);
                    ParserContext.Current.AddPackage(new SchemaFile(packageName, 1));

                    using StreamReader reader = OpenFile(file);
                    await mParser.ParseFile(reader, packageName);
                }

            }
            else
            {
                if (Path.GetExtension(path) != Keywords.FileExtension)
                    throw new FileLoadException("Invalid file extension.", path);

                string packageName = Path.GetFileNameWithoutExtension(path);
                ParserContext.Current.AddPackage(new SchemaFile(packageName, 1));

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