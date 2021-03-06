using MessagePack;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Plugin.SchemaEncoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin.Encoder
{
    public class MessagePackSchemaEncoder : ISchemaEncoder
    {
        public async Task<ReadOnlyMemory<byte>> Encode(PluginEncoding encoding, IEnumerable<SchemaPackage> packages)
        {
            var schema = new PluginInterpretedSchema { Encoding = encoding };
            foreach(SchemaPackage package in packages)
            {
                Dictionary<string, object> packagemap = new()
                {
                    { "id", package.Id },
                    { "version", package.Version },
                    { "name", package.Name },
                    { "imports", package.Imports }
                };

                List<object> types = new(package.Types.Count);
                foreach(SchemaType type in package.Types)
                {
                    Dictionary<string, object> typeMap = new()
                    {
                        { "id", type.Id },
                        { "name", type.Name },
                        //{ "package", type.Package },
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
                            { "type", WriteValueType(field.ValueType) },
                            { "defaultValue", SerializeDefaultValue(field.DefaultValue) },
                            { "metadata", field.Metadata }
                        };

                        fields.Add(fieldMap);
                    }

                    typeMap["fields"] = fields;
                    types.Add(typeMap);
                }

                packagemap["types"] = types;
                schema.Files.Add(packagemap);
            }

            using MemoryStream stream = new();
            await MessagePackSerializer.SerializeAsync(stream, schema);

            return new ReadOnlyMemory<byte>(stream.GetBuffer());
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
    
        public static object SerializeDefaultValue(object defaultValue)
        {
            if (defaultValue is CustomTypeValue customTypeValue)
                return new Dictionary<string, object>
                {
                    { "typeId", customTypeValue.TypeId },
                    { "value", customTypeValue.Value }
                };
            else
                return defaultValue;
        }
    }
}