using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            Binary,
        };

        public static readonly IDictionary<string, int> TypeCodes = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()
        {
            { String, 1 },
            { Uint8, 2 },
            { Uint16, 3 },
            { Uint32, 4 },
            { Uint64, 5 },
            { Int8, 6 },
            { Int16, 7 },
            { Int32, 8 },
            { Int64, 9 },
            { Float32, 10 },
            { Float64, 11 },
            { Boolean, 12 },
            { Binary, 13 },
            { List, 14 },
            { Map, 15 },
            { Custom, 16 },
        });
    }
}