using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Plugin.SchemaEncoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin.Encoder
{
    public class JsonSchemaEncoder : ISchemaEncoder
    {
        public async Task<ReadOnlyMemory<byte>> Encode(string outputPath, IEnumerable<SchemaFile> files)
        {
            var schema = new PluginSendSchema();
            foreach (SchemaFile file in files)
            {
                Dictionary<string, object> fileMap = new()
                {
                    { "version", file.Version },
                    { "name", file.Name },
                };

                List<object> types = new(file.Types.Count);
                foreach(SchemaType type in file.Types)
                {
                    Dictionary<string, object> typeMap = new()
                    {
                        { "id", type.Id },
                        { "name", type.Name },
                        { "package", type.Package },
                        { "fullName", type.FullName },
                        { "modifier", WriteModifier(type.Modifier) },
                    };

                    List<object> fields = new(type.Fields.Count);
                    foreach(SchemaTypeField field in type.Fields)
                    {
                        Dictionary<string, object> fieldMap = new()
                        {
                            { "name", field.Name },
                            { "index", field.Index },
                            { "isNullable", field.IsNullable },
                            { "type", WriteValueType(field.ValueType) },
                            { "defaultValue", field.DefaultValue },
                            { "metadata", field.Metadata }
                        };

                        fields.Add(fieldMap);
                    }

                    typeMap["fields"] = fields;
                    types.Add(typeMap);
                }

                fileMap["types"] = types;
                schema.Types.Add(fileMap);
            }

            using MemoryStream stream = new();
            await JsonSerializer.SerializeAsync(stream, schema);

            return new ReadOnlyMemory<byte>(stream.GetBuffer());
        }

        private static string WriteModifier(SchemaTypeModifier? modifier)
        {
            if (modifier == null)
                return null;

            return modifier switch
            {
                SchemaTypeModifier.Enum => "enum",
                _ => throw Check.Internal("Invalid type modifier")
            };
        }

        public static object WriteValueType(SchemaTypeFieldValueType valueType)
        {
            if (valueType is PrimitiveSchemaFieldValueType primitive)
                return new Dictionary<string, object>
                {
                    { "dataType", "primitive" },
                    { "type", primitive.TypeName }
                };
            else if (valueType is CustomSchemaFieldValueType custom)
                return new Dictionary<string, object>
                {
                    { "dataType", custom.TypeName },
                    { "type", custom.CustomType }
                };
            else if (valueType is ListSchemaFieldValueType list)
                return new Dictionary<string, object>
                {
                    { "dataType", "list" },
                    { "elementType", list.ElementType.TypeName }
                };
            else if (valueType is MapSchemaFieldValueType map)
                return new Dictionary<string, object>
                {
                    { "dataType", "map" },
                    { "keyType", map.KeyType.TypeName },
                    { "valueType", map.ValueType.TypeName },
                };
            else
                return null;
        }
    }
}