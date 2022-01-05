using MessagePack;
using MessagePack.Resolvers;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin.Encoder
{
    public class MessagePackSchemaEncoder : ISchemaEncoder
    {
        public async Task<ReadOnlyMemory<byte>> Encode(IEnumerable<SchemaFile> files)
        {
            List<object> entries = new();
            foreach(SchemaFile file in files)
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
                        { "modifier", type.Modifier },
                    };

                    List<object> fields = new(type.Fields.Count);
                    foreach(SchemaTypeField field in type.Fields)
                    {
                        Dictionary<string, object> fieldMap = new()
                        {
                            { "name", field.Name },
                            { "index", field.Index },
                            { "isNullable", field.IsNullable },
                            { "valueType", WriteValueType(field.ValueType) },
                            { "defaultValue", field.DefaultValue },
                            { "metadata", field.Metadata }
                        };

                        fields.Add(fieldMap);
                    }

                    typeMap["fields"] = fields;
                    types.Add(typeMap);
                }

                fileMap["types"] = types;
                entries.Add(fileMap);
            }

            using MemoryStream stream = new();
            await MessagePackSerializer.SerializeAsync(stream, entries, ContractlessStandardResolver.Options);

            return new ReadOnlyMemory<byte>(stream.GetBuffer());
        }

        public static object WriteValueType(SchemaTypeFieldValueType valueType)
        {
            if (valueType is PrimitiveSchemaFieldValueType primitive)
                return new Dictionary<string, object>
                {
                    { "valueType", "primitive" },
                    { "type", primitive.TypeName }
                };
            else if (valueType is CustomSchemaFieldValueType custom)
                return new Dictionary<string, object>
                {
                    { "valueType", custom.TypeName },
                    { "type", custom.CustomType }
                };
            else if (valueType is ListSchemaFieldValueType list)
                return new Dictionary<string, object>
                {
                    { "valueType", "list" },
                    { "elementType", list.ElementType.TypeName }
                };
            else if (valueType is MapSchemaFieldValueType map)
                return new Dictionary<string, object>
                {
                    { "valueType", "map" },
                    { "keyType", map.KeyType.TypeName },
                    { "valueType", map.ValueType.TypeName },
                };
            else
                return valueType.TypeName;
        }
    }
}