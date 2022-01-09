using MessagePack;

namespace MessagePackSchema.CsharpGenerator
{
    internal class FileOutput
    {
        public string Path { get; }

        public ReadOnlyMemory<byte> Data { get; }

        public FileOutput(string path, ReadOnlyMemory<byte> data)
        {
            Path = path;
            Data = data;
        }
    }

    internal class SchemaFile
    {
        [Key("version")]
        public string? Version { get; set; }

        [Key("name")]
        public string? Name { get; set; }

        [Key("types")]
        public List<SchemaType>? Types { get; set; }
    }

    internal class SchemaType
    {
        [Key("id")]
        public string? Id { get; set; }

        [Key("name")]
        public string? Name { get; set; }

        [Key("package")]
        public string? Package { get; set; }

        [Key("fullName")]
        public string? FullName { get; set; }

        [Key("modifier")]
        public int? Modifier { get; set; }

        [Key("fields")]
        public List<SchemaField>? Fields { get; set; }
    }

    internal class SchemaField
    {
        [Key("index")]
        public int? Index { get; set; }

        [Key("name")]
        public string? Name { get; set; }

        [Key("isNullable")]
        public bool IsNullable { get; set; }

        [Key("type")]
        public int? FullName { get; set; }

        [Key("modifier")]
        public int? Modifier { get; set; }

        [Key("type")]
        public SchemaFieldValueType? ValueType { get; set; }

        [Key("defaultValue")]
        public object? DefaultValue { get; set; }

        [Key("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }

    internal class SchemaFieldValueType
    {
        [Key("dataType")]
        public string? DataType { get; set; }

        [Key("typeName")]
        public string? TypeName { get; set; }

        [Key("elementType")]
        public string? ElementType { get; set; }

        [Key("keyType")]
        public string? KeyType { get; set; }

        [Key("valueType")]
        public string? ValueType { get; set; }

    }
}