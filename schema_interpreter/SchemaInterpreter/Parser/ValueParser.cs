using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser
{
    public static class ValueParser
    {
        public static object Parse(string input, SchemaTypeFieldValueType valueType)
            => ParseType(input, valueType, false);

        private static object ParseType(string input, SchemaTypeFieldValueType valueType, bool isNested)
        {
            Logger.Debug($"Parsing input [{input}] to [{valueType.TypeName}]");

            return valueType.TypeName switch
            {
                SchemaFieldValueTypes.String => input.Replace("\"", ""),
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
                SchemaFieldValueTypes.Binary => throw Check.InvalidSchema("binary fields cannot declare default values"),
                SchemaFieldValueTypes.List => isNested ? ThrowIsNested() : ParseList(input, valueType),
                SchemaFieldValueTypes.Map => isNested ? ThrowIsNested() : ParseMap(input, valueType),
                SchemaFieldValueTypes.Custom => ParseCustom(input),
                _ => throw new InvalidSchemaException($"Value type '{valueType.TypeName}' could not be recognized.")
            };
        }

        private static List<object> ParseList(string input, SchemaTypeFieldValueType valueType)
        {
            if (valueType is not ListSchemaFieldValueType listValue)
            {
                Check.ThrowInvalidSchema("Default value for field is not a list.");
                return null;
            }

            string[] values = input.Replace("[", "").Replace("]", "").Split(',', StringSplitOptions.TrimEntries);
            return values.Select(value => Parse(value, listValue.ElementType)).ToList();
        }

        private static Dictionary<object, object> ParseMap(string input, SchemaTypeFieldValueType valueType)
        {
            if (valueType is not MapSchemaFieldValueType mapValue)
            {
                Check.ThrowInvalidSchema("Default value for field is not a map.");
                return null;
            }

            IEnumerable<string[]> values = input.Replace("[", "").Replace("]", "")
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(pair => pair.Replace("(", "").Replace(")", "").Split(":"));

            return values.ToDictionary(key => Parse(key[0], mapValue.KeyType), value => Parse(value[1], mapValue.ValueType));
        }

        private static CustomTypeValue ParseCustom(string input)
        {
            string[] tokens = input.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2 || tokens.Length > 3)
                Check.ThrowInvalidSchema("Invalid custom type default value.");

            string package = null;
            string typeName;
            string value;

            if (tokens.Length == 3)
            {
                package = tokens[0];
                typeName = tokens[1];
                value = tokens[2];
            }
            else
            {
                typeName = tokens[0];
                value = tokens[1];
            }

            string typeId = CommonHelpers.CalculateMD5(typeName);

            // TODO: add check to disallow default values when struct or union.
            ParserContext.Current.EnsureTypeId(typeId, typeName, package, CommonHelpers.CalculateMD5(package));
            ParserContext.Current.EnsureTypeValue(value, typeId, typeName, package);

            return new CustomTypeValue(typeId, value);
        }

        private static object ThrowIsNested()
        {
            Check.ThrowInvalidSchema("List and Map types cannot be nested");
            return null;
        }
    }
}