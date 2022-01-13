using SchemaInterpreter.Parser.Definition;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SchemaInterpreter.Test
{
    public static class TestUtils
    {
        public static StreamReader GetTestFile(string name) => new(File.OpenRead(Path.Combine("TestFiles", name)));

        public static SchemaPackageBuilder GetBuilder() => new();
    }

    public class SchemaPackageBuilder
    {
        private StringBuilder mWriter;

        public SchemaPackageBuilder()
        {
            mWriter = new();
        }

        public SchemaPackageBuilder Write(string line)
        {
            mWriter.AppendLine(line);
            return this;
        }

        public SchemaPackageBuilder WriteVersion(int version) => Write($"version:{version}");

        public SchemaPackageBuilder WriteComment(string comment) => Write($"//{comment}");
        
        public SchemaPackageBuilder WriteImport(string import) => Write($"import \"{import}\"");

        public SchemaPackageBuilder WriteType(SchemaType type)
        {
            Write($"type {type.Name} {{");
            foreach(var field in type.Fields)
            {
                string declaration = $"{field.Name}{(field.IsNullable ? "?" : "")}:{field.ValueType} {field.Index}";
                if(field.DefaultValue != null)
                    declaration += $" = {WriteDefaultValue(field.DefaultValue)}";

                if (field.Metadata != null)
                    declaration += $"@{WriteDefaultValue(field.Metadata)}";

                Write(declaration);
            }

            Write("}");
            return this;
        }

        private string WriteDefaultValue(object value)
        {
            if (value is string)
                return $"\"{value}\"";
            else if (value is IList enumerable)
                return $"({enumerable.Cast<object>().Select(x => WriteDefaultValue(x))})";
            else if (value is IEnumerable<KeyValuePair<object, object>> dictionary)
                return $"({dictionary.Select(x => $"[{WriteDefaultValue(x.Key)}:{WriteDefaultValue(x.Value)}],")})";
            else
                return value.ToString();
        }

        /// <summary>
        /// Builds the reader to output to the parser.
        /// </summary>
        public StreamReader BuildReader()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(mWriter.ToString());
            writer.Flush();
            stream.Position = 0;
            return new StreamReader(stream);
        }

        public override string ToString()
        {
            using var reader = BuildReader();
            return reader.ReadToEnd();
        }
    }
}