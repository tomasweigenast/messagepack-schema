using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser
{
    public static class ValueParser
    {
        public static object Parse(string input, SchemaTypeFieldValueType valueType, int lineIndex, string line)
            => ParseType(input, valueType, lineIndex, line, false);

        private static object ParseType(string input, SchemaTypeFieldValueType valueType, int lineIndex, string line, bool isNested)
            => valueType.TypeName switch
            {
                SchemaFieldValueTypes.String => input.Replace('"', '\0'),
                SchemaFieldValueTypes.Boolean => bool.Parse(input),
                SchemaFieldValueTypes.Float32 => float.Parse(input),
                SchemaFieldValueTypes.Float64 => double.Parse(input),
                SchemaFieldValueTypes.Uint8 => byte.Parse(input),
                SchemaFieldValueTypes.Int8 => sbyte.Parse(input),
                SchemaFieldValueTypes.Uint16 => ushort.Parse(input),
                SchemaFieldValueTypes.Int16 => short.Parse(input),
                SchemaFieldValueTypes.Uint32 => uint.Parse(input),
                SchemaFieldValueTypes.Int32 => int.Parse(input),
                SchemaFieldValueTypes.Uint64 => ulong.Parse(input),
                SchemaFieldValueTypes.Int64 => long.Parse(input),
                SchemaFieldValueTypes.Binary => Convert.FromBase64String(input),
                SchemaFieldValueTypes.List => isNested ? ThrowIsNested(lineIndex, line) : ParseList(input, valueType, lineIndex, line),
                SchemaFieldValueTypes.Map => isNested ? ThrowIsNested(lineIndex, line) : ParseMap(input, valueType, lineIndex, line),
                SchemaFieldValueTypes.Custom => ParseCustom(input, lineIndex, line),
                _ => throw new InvalidSchemaException($"Value type '{valueType.TypeName}' could not be recognized.", lineIndex, line)
            };

        private static List<object> ParseList(string input, SchemaTypeFieldValueType valueType, int lineIndex, string line)
        {
            if (valueType is not ListSchemaFieldValueType listValue)
                throw new InvalidSchemaException("Default value for field is not a list.", lineIndex, line);

            string[] values = input.Replace("[", "").Replace("]", "").Split(',', StringSplitOptions.TrimEntries);
            return values.Select(value => Parse(value, listValue.ElementType, lineIndex, line)).ToList();
        }

        private static Dictionary<object, object> ParseMap(string input, SchemaTypeFieldValueType valueType, int lineIndex, string line)
        {
            if (valueType is not MapSchemaFieldValueType mapValue)
                throw new InvalidSchemaException("Default value for field is not a map.", lineIndex, line);

            IEnumerable<string[]> values = input.Replace("[", "").Replace("]", "")
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(pair => pair.Replace("(", "").Replace(")", "").Split(","));

            return values.ToDictionary(key => Parse(key[0], mapValue.KeyType, lineIndex, line), value => Parse(value[0], mapValue.ValueType, lineIndex, line));
        }

        private static CustomTypeValue ParseCustom(string input, int lineIndex, string line)
        {
            string[] tokens = input.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
                throw new InvalidSchemaException("Invalid custom type default value.", lineIndex, line);

            return new CustomTypeValue(tokens[0].GetHashCode(), tokens[0]);
        }

        private static InvalidSchemaException ThrowIsNested(int lineIndex, string line)
            => throw new InvalidSchemaException("List and Map types cannot be nested.", lineIndex, line);
    }
}