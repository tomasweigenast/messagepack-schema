using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Builder;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SchemaInterpreter.Test")]
namespace SchemaInterpreter.Parser.V1
{
    public class SchemaFileParserV1 : ISchemaFileParser
    {
        private static readonly int mCurrentVersion = 1;
        private static readonly Regex mTypeNameRegex = new("[0-9A-Z_][a-z]*([0-9A-Z_][a-z]*)*", RegexOptions.Compiled);

        public int Version => 1;

        public async Task ParseFile(StreamReader reader, string packageName)
        {
            // Convert package names to lowercase
            packageName = packageName.ToLower();

            string line;
            int lineIndex = 0;
            int? version = null;
            bool startReadingBody = false;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Sanitize line
                lineIndex++;
                line = line.Replace("\t", "").Replace("\r", "").Trim();
                
                ParserContext.Current.CurrentLine = new FileLine(lineIndex, line, packageName);

                Logger.Debug("Removed carriage and tabs.");

                // Remove any comment from the line
                int commentIndex = line.IndexOf("//");
                if(commentIndex != -1)
                {
                    line = line.Remove(commentIndex, line.Length - commentIndex).Trim();
                    Logger.Debug($"Removed comment at index {commentIndex}");
                }

                // Skip empty lines
                if (line.IsNullOrWhitespace())
                    continue;

                // Start interpreting
                if(line.StartsWith("version:"))
                {
                    version = ReadVersion(line);
                    ParserContext.Current.CurrentPackage = new SchemaPackage(packageName, (int)version);
                    Logger.Debug("Version read.");
                    continue;
                }
                else if (line.StartsWith("import "))
                {
                    if (startReadingBody)
                        Check.ThrowInvalidSchema("Imports must be declared before any type.");

                    // Read import statements
                    try
                    {
                        string importedPackage = line.Split("import", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\"", "").Trim()).ToArray()[0];
                        bool added = ParserContext.Current.CurrentPackage.Imports.Add(importedPackage.ToLower());
                        if (!added)
                            Check.ThrowInvalidSchema($"Package {importedPackage} is already imported.");
                    }
                    catch
                    {
                        Check.ThrowInvalidSchema("Import statement must be followed by the package name to import.");
                    }

                }
                else
                {
                    if (version == null)
                        Check.ThrowInvalidSchema("Schema does not specify a version. It must be the first line to be set.");
                    else
                    {
                        startReadingBody = true;

                        char lastLineChar = line[^1];
                        switch (lastLineChar)
                        {
                            // Starts a new type
                            case '{':
                                Logger.Debug("Detected new type. Reading...");

                                if (ParserContext.Current.CurrentTypeBuilder != null)
                                    Check.ThrowInvalidSchema("When creating a type, closing tags should be present before creating a new one.");

                                var (typeName, modifier) = ReadTypeNameAndModifier(line);
                                ParserContext.Current.CurrentTypeBuilder = new SchemaTypeBuilder(typeName, packageName, modifier);
                                break;

                            // Ends a type.
                            case '}':
                                Logger.Debug("Detected end of new type. Building...");

                                if (ParserContext.Current.CurrentTypeBuilder == null)
                                    Check.ThrowInvalidSchema("Before closing a type, a new one must be created.");

                                ParserContext.Current.BuildCurrentTypeBuilder();
                                break;

                            default:
                                if (ParserContext.Current.CurrentTypeBuilder == null)
                                    Check.ThrowInvalidSchema("Before specifying a field, a type must be created.");

                                Logger.Debug("Reading type field...");

                                var typeField = ReadField(line);
                                ParserContext.Current.CurrentTypeBuilder.AddField(typeField);
                                break;
                        }
                    }
                }

            }
        }
        
        /// <summary>
        /// Reads a field from a line.
        /// </summary>
        internal static SchemaTypeField ReadField(string input)
        {
            string fieldName = null;
            SchemaTypeFieldValueType fieldType = null;
            int? fieldIndex = null;
            string rawDefaultValue = null;
            IDictionary<string, object> metadata = null;
            SchemaTypeModifier? modifier = ParserContext.Current.CurrentTypeBuilder.Modifier;

            string[] tokens = input.Split(modifier != SchemaTypeModifier.Enum ? ":" : " ", 2, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            // Read field name
            fieldName = tokens[0];

            // Check if the field is not null
            if (fieldName.IsNullOrWhitespace())
                Check.ThrowInvalidSchema("Fields must specify a name.");

            // If the current type is an enum, simply read its field index and continue.
            if(modifier == SchemaTypeModifier.Enum)
            {
                if (tokens.Length != 2)
                    Check.ThrowInvalidSchema("Enum fields should only specify a name and an index.");

                if (fieldName.Contains(':'))
                    Check.ThrowInvalidSchema("Enum fields cannot declare value types.");

                fieldIndex = TypeParser.Int(tokens[1], $"Invalid field index as int. Given: {tokens[1]}");
            }
            else
            {
                string metadataAndDefaultValue = null;

                // Check if metadata or default value is specified in the line. If so, substring and remove from line.
                int metadataOrDefaultValueIndex = tokens[1].IndexOf(new char[] { '=', '@' });
                if(metadataOrDefaultValueIndex != -1)
                {
                    metadataAndDefaultValue = tokens[1][metadataOrDefaultValueIndex..];
                    tokens[1] = tokens[1].Remove(metadataOrDefaultValueIndex).Trim();
                }

                // Split tokens again, but this time by whitespace
                // Here, tokens[1] contains the field type and its index.
                int lastIndexOfWhitespace = tokens[1].LastIndexOf(" ");

                // avoid fields that doesnt have a schema field.
                if (lastIndexOfWhitespace == -1)
                    Check.ThrowInvalidSchema("Schema field should specify an index.");

                // Read field type and field index
                string rawFieldType = tokens[1].Substring(0, lastIndexOfWhitespace);
                string rawFieldIndex = tokens[1][(lastIndexOfWhitespace + 1)..];

                fieldType = GetValueType(rawFieldType);
                fieldIndex = TypeParser.Int(rawFieldIndex, $"Expect field index as int. Given: {tokens[1]}");

                if(metadataAndDefaultValue != null)
                {
                    // .Split(new string[] { "=", "@" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    // here its split by both characters (= and @) because if both default value and metadata is defined, default value must go first, so its easy to check

                    char? firstChar = metadataAndDefaultValue.FindFirst(new char[] { '=', '@' });
                    if (firstChar == null)
                        Check.ThrowInvalidSchema("Default values and metadata must start with '=' and '@', respectively");

                    switch (firstChar)
                    {
                        case '@':
                            // Metadata should go after default values.
                            if(metadataAndDefaultValue.Contains('@') && metadataAndDefaultValue.Contains('='))
                                Check.ThrowInvalidSchema("When defining default value and metadata for a field, default values must be specified first.");

                            // Read metadata
                            metadata = ReadMetadata(metadataAndDefaultValue.Remove(0, 1).Trim());

                            break;

                        case '=':
                            string[] metadataAndDefaultValueTokens = metadataAndDefaultValue.Split("@", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Remove(0, 1).Trim()).ToArray();
                            rawDefaultValue = metadataAndDefaultValueTokens[0];

                            if (metadataAndDefaultValueTokens.Length > 2)
                                Check.ThrowInvalidSchema("Invalid characters after metadata or default value.");
                            else if(metadataAndDefaultValueTokens.Length == 2)
                                metadata = ReadMetadata(metadataAndDefaultValueTokens[1]);

                            break;
                    }
                }
            }

            // Check if the field is nullable
            bool isNullable = fieldName[^1] == '?';
            if (isNullable)
                if (modifier == SchemaTypeModifier.Enum || modifier == SchemaTypeModifier.Union)
                    Check.ThrowInvalidSchema("Enums and Unions cant declare nullable fields.");
                else
                    fieldName = fieldName[0..^1];

            ParserContext.Current.EnsureEmptyTypeFieldIndex(fieldIndex.Value);
            ParserContext.Current.EnsureEmptyTypeFieldName(fieldName);

            return new SchemaTypeField(fieldName, fieldIndex.Value, fieldType, rawDefaultValue, isNullable, metadata, ParserContext.Current.CurrentLine);
        }

        internal static SchemaTypeFieldValueType GetValueType(string input)
        {
            string[] tokens = input.Replace(")", "").Split("(", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            string valueType = tokens[0]; // value type, for example: list
            string elementType = tokens.Length == 2 ? tokens[1] : null; // element type, for example, string, or string,int

            if (tokens.Length > 2)
                Check.ThrowInvalidSchema("Nested list or maps is not allowed.");

            if (elementType == null)
                return GetPrimitiveOrCustomValueType(valueType);
            else
            {
                string[] elementTypeTokens = elementType.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (valueType == SchemaFieldValueTypes.List)
                    if (elementTypeTokens.Length == 1)
                    {
                        var listElementType = GetPrimitiveOrCustomValueType(elementType.Trim());
                        EnsureListMapValidType(listElementType);
                        return SchemaTypeFieldValueType.List(listElementType);
                    }
                    else
                        Check.ThrowInvalidSchema("List types can only specify element type, not key-value pairs.");
                else if (valueType == SchemaFieldValueTypes.Map)
                    if (elementTypeTokens.Length == 2)
                    {
                        var mapKeyType = GetPrimitiveOrCustomValueType(elementTypeTokens[0].Trim());
                        var mapValueType = GetPrimitiveOrCustomValueType(elementTypeTokens[1].Trim());

                        EnsureListMapValidType(mapKeyType);
                        EnsureListMapValidType(mapValueType);

                        return SchemaTypeFieldValueType.Map(mapKeyType, mapValueType);
                    }
                    else
                        Check.ThrowInvalidSchema("Map types should specify key and value types.");
                else
                    Check.ThrowInvalidSchema("Only list and map values can have element types.");
            }

            return null;
        }

        internal static SchemaTypeFieldValueType GetPrimitiveOrCustomValueType(string valueType)
        {
            if (SchemaFieldValueTypes.Primitives.Any(x => x == valueType))
                return SchemaTypeFieldValueType.Primitive(valueType);
            else
                if (mTypeNameRegex.IsMatch(valueType.Trim()))
                    return SchemaTypeFieldValueType.Custom(valueType);
                else
                {
                    Check.ThrowInvalidSchema($"Unknown primitive type {valueType}.");
                    return null;
                }
        }

        internal static (string typeName, SchemaTypeModifier? modifier) ReadTypeNameAndModifier(string input)
        {
            string[] parts = input.Split(" ", StringSplitOptions.TrimEntries).TakeWhile(x => x != Keywords.StartNewType).ToArray();
            if (parts.Length > 3 || parts.Length < 2)
                Check.ThrowInvalidSchema("Invalid type declaration.");

            string typeKeyword = parts[0];
            if (typeKeyword != Keywords.Type)
                Check.ThrowInvalidSchema($"Expected 'type' keyword, given '{typeKeyword}'");

            string typeName = parts[1];
            if (!mTypeNameRegex.IsMatch(typeName))
                Check.ThrowInvalidSchema($"Type name does not match expected regex: '^[a-zA-Z_]*$'");

            SchemaTypeModifier? modifier = null;
            if (parts.Length == 3)
                modifier = parts[2].ToSchemaTypeModifier();

            return (typeName, modifier);
        }

        internal static IDictionary<string, object> ReadMetadata(string input)
        {
            static object ParseMetadataValue(string input)
            {
                if (input.Contains('"'))
                    return input.Replace("\"", string.Empty);
                else if (bool.TryParse(input, out bool boolValue))
                    return boolValue;
                else if (long.TryParse(input, out long longValue))
                    return longValue;
                else if (double.TryParse(input, out double doubleValue))
                    return doubleValue;
                else
                    Check.ThrowInvalidSchema("Metadata values can be only of string, boolean, int or float types.");

                return null;
            }

            Dictionary<string, object> metadata = new();
            var entries = input
                .Replace("(", "")
                .Replace(")", "")
                .Replace(Keywords.Metadata, string.Empty)
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x
                    .Replace("]", "")
                    .Replace("[", "")
                    .Trim());

            foreach (string entry in entries)
            {
                string[] tokens = entry.Split(":");
                if (tokens.Length != 2)
                    Check.ThrowInvalidSchema("Metadata entries should contain exactly two values.");

                string key = tokens[0];
                if (key.StartsWith('"') && key.EndsWith('"'))
                    key = key.Replace("\"", string.Empty);
                else
                    Check.ThrowInvalidSchema("Metadata keys can be only of type string.");

                object value = ParseMetadataValue(tokens[1]);

                metadata[key] = value;
            }

            return metadata;
        }

        internal static int ReadVersion(string input)
        {
            try
            {
                int version = int.Parse(input.Split("version:")[1]);
                if (version != mCurrentVersion)
                    Check.ThrowInvalidSchema($"Invalid version. Expected: 1 - Given: {version}");

                return version;
            }
            catch
            {
                Check.ThrowInvalidSchema("Invalid version declaration.");
                return 0;
            }
        }

        internal static void EnsureListMapValidType(SchemaTypeFieldValueType valueType)
        {
            int typeCode = valueType.TypeCode;
            int mapTypeCode = SchemaFieldValueTypes.TypeCodes[SchemaFieldValueTypes.Map];
            int listTypeCode = SchemaFieldValueTypes.TypeCodes[SchemaFieldValueTypes.List];
            if (typeCode == mapTypeCode || typeCode == listTypeCode)
                Check.ThrowInvalidSchema("Map and lists cannot store other lists or maps");
        }
    }
}