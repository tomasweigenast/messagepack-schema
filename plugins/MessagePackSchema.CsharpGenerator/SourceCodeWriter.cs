using CsCodeGenerator;
using CsCodeGenerator.Enums;

namespace MessagePackSchema.CsharpGenerator
{
    /// <summary>
    /// Provides methods to write source code files.
    /// </summary>
    public class SourceCodeWriter
    {
        public static IEnumerable<FileOutput> GenerateFiles(IEnumerable<SchemaFile> files)
        {
            foreach(SchemaFile file in files)
            {

            }
        }

        private static FileOutput GenerateFile(SchemaFile file)
        {
            var types = file.Types?.First();

            var usingDirectives = new string[]
            {
                "using System;",
                "using MessagePack;",
                "using MessagePackSchema.Runtime;",
            };

            string fileNamespace = $"MessagePackSchema.Generated";

            var classModel = GetClassModel(file);
        }

        private static ClassModel GetClassModel(SchemaFile file)
        {
            return new ClassModel(file.Name)
            {
                KeyWords = new List<KeyWord>()
                {
                    KeyWord.Partial,
                },
                Interfaces = new List<string>()
                {
                    $"ISchemaType<{file.Name}>",
                },
                DefaultConstructor = new Constructor(file.Name)
                {
                    IsVisible = true,
                    AccessModifier = AccessModifier.Public,
                    BodyLines = new List<string>()
                    {
                        "OnCreate();"
                    }
                }
            };
        }

        private static IList<Field> GetFields(SchemaFile file)
        {
            var list = new List<Field>();

            foreach(var )

            return list;
        }
    }
}