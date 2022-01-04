namespace SchemaInterpreter.Parser.Definition
{
    public static class SchemaFieldValueTypes
    {
        public const string String = "string";
        public const string Boolean = "boolean";
        public const string Float32 = "float32";
        public const string Float64 = "float64";
        public const string Uint8 = "uint8";
        public const string Uint16 = "uint16";
        public const string Uint32 = "uint32";
        public const string Uint64 = "uint64";
        public const string Int8 = "int8";
        public const string Int16 = "int16";
        public const string Int32 = "int32";
        public const string Int64 = "int64";
        public const string Binary = "binary";
        public const string List = "list";
        public const string Map = "map";
        public const string Custom = "custom";

        public static readonly string[] Values =
        {
            String,
            Boolean,
            Float32,
            Float64,
            Uint8,
            Uint16,
            Uint32,
            Uint64,
            Int8,
            Int16,
            Int32,
            Int64,
            Binary,
            List,
            Map,
            Custom,
        };

        public static readonly string[] Primitives =
        {
            String,
            Boolean,
            Float32,
            Float64,
            Uint8,
            Uint16,
            Uint32,
            Uint64,
            Int8,
            Int16,
            Int32,
            Int64,
        };
    }
}