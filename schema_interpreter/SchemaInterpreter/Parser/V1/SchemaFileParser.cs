using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchemaInterpreter.Parser.V1
{
    public class SchemaFileParser : ISchemaFileParser
    {
        private static readonly int mCurrentVersion = 1;
        private static readonly Regex mTypeNameRegex = new("^[a-zA-Z_]*$", RegexOptions.Compiled);

        public int Version => 1;

        public async Task ParseFile(StreamReader reader, string packageName)
        {
            string line;
            int lineIndex = 0;
            bool commentStarted = false;
            int? version = null;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineIndex++;
                line = line.Replace("\t", "").Replace("\r", "").Trim();
                ParserContext.Current.CurrentLine = new FileLine(lineIndex, line, packageName);

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                Logger.Debug("Removed carriage and tabs.");

                // Ignore comments
                if (line.StartsWith('/'))
                {
                    commentStarted = true;
                    Logger.Debug("Comment started.");
                    continue;
                }

                // Ingnore double line comment
                else if (line.StartsWith("*"))
                    if (commentStarted)
                    {
                        Logger.Debug("Multine comment is following.");
                        continue;
                    }
                    else
                        Check.ThrowInvalidSchema("Multiline comments should start with a slash: '/'");

                // Start interpreting
                else
                {
                    if (line.StartsWith("version:"))
                    {
                        version = ReadVersion(line);
                        continue;
                    }
                    else
                    {
                        if (version == null)
                            Check.ThrowInvalidSchema("Schema does not specify a version. It must be the first line to be set.");
                        else
                        {
                            char lastLineChar = line[^1];
                            switch (lastLineChar)
                            {
                                // Starts a new type
                                case '{':
                                    if (ParserContext.Current.CurrentTypeBuilder != null)
                                        Check.ThrowInvalidSchema("When creating a type, closing tags should be present before creating a new one.");

                                    var (typeName, modifier) = ReadTypeNameAndModifier(line);
                                    ParserContext.Current.EnsureEmptyTypeName(typeName);
                                    ParserContext.Current.CurrentTypeBuilder = new SchemaTypeBuilder(typeName, packageName, modifier);
                                    break;

                                // Ends a type.
                                case '}':
                                    if (ParserContext.Current.CurrentTypeBuilder == null)
                                        Check.ThrowInvalidSchema("Before closing a type, a new one must be created.");

                                    ParserContext.Current.Add(ParserContext.Current.CurrentTypeBuilder.Build(), packageName);
                                    ParserContext.Current.CurrentTypeBuilder = null;
                                    break;

                                default:
                                    if (ParserContext.Current.CurrentTypeBuilder == null)
                                        Check.ThrowInvalidSchema("Before specifying a field, a type must be created.");

                                    var typeField = ReadTypeField(line);
                                    ParserContext.Current.CurrentTypeBuilder.AddField(typeField);
                                    break;
                            }
                        }
                    }
                }

            }
        }

        private static SchemaTypeField ReadTypeField(string input)
        {
            string fieldName = null;
            SchemaTypeFieldValueType fieldType = null;
            int? fieldIndex = null;
            string rawDefaultValue = null;
            IDictionary<string, object> metadata = null;
            SchemaTypeModifier? modifier = ParserContext.Current.CurrentTypeBuilder.Modifier;

            string[] tokens = input.Split(modifier == null ? ":" : " ", 2, StringSplitOptions.RemoveEmptyEntries);
            fieldName = tokens[0].Trim();

            if(modifier == SchemaTypeModifier.Enum)
            {
                fieldIndex = TypeParser.Int(tokens[1], "Invalid field index.");
            }
            else
            {
                tokens = tokens[1].Split(" ", 2, StringSplitOptions.TrimEntries);
                fieldType = GetValueType(tokens[0]);

                tokens = tokens[1].Split(new string[] { "=", "@" }, StringSplitOptions.TrimEntries);
                fieldIndex = TypeParser.Int(tokens[0], "Invalid field index.");

                if (tokens.Length > 1 && tokens.Length <= 3)
                {
                    if (tokens.Length == 3)
                    {
                        rawDefaultValue = tokens[1];
                        metadata = ReadMetadata(tokens[2]);
                    }
                    else if (tokens.Length == 2)
                    {
                        string firstToken = tokens[1];
                        if (firstToken.StartsWith('('))
                            metadata = ReadMetadata(firstToken);
                        else
                            rawDefaultValue = firstToken;
                    }
                }
            }

            bool isNullable = fieldName[^1] == '?';
            if (isNullable)
                if (modifier == SchemaTypeModifier.Enum)
                    Check.ThrowInvalidSchema("Enums cant have nullable fields.");
                else
                    fieldName = fieldName[0..^1];

            ParserContext.Current.EnsureEmptyTypeName(fieldName);
            ParserContext.Current.EnsureEmptyTypeFieldIndex(fieldIndex.Value);

            return new SchemaTypeField(fieldName, fieldIndex.Value, fieldType, rawDefaultValue, isNullable, metadata, ParserContext.Current.CurrentLine);
        }

        private static SchemaTypeField ReadTypeField_(string input)
        {
            // If the input contains metadata
            string metadataInput = null;
            if(input.Contains(Keywords.Metadata))
            {
                int indexOfMetadata = input.IndexOf(Keywords.Metadata);
                metadataInput = input[indexOfMetadata..];
                input = input.Replace(metadataInput, "");
            }

            // Get a default value if it contains it
            string rawDefaultValue = null;
            if (input.Contains(Keywords.DefaultValue))
            {
                int indexOfDefaultValue = input.IndexOf(Keywords.DefaultValue);
                rawDefaultValue = input[indexOfDefaultValue..];
                input = input.Replace(rawDefaultValue, "");
            }

            var modifier = ParserContext.Current.CurrentTypeBuilder.Modifier;
            string[] splitChars = ParserContext.Current.CurrentTypeBuilder.Modifier == null ? new string[] { ":", " " } : new string[] { " " };
            string[] tokens = input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            if (modifier == null && tokens.Length < 3)
                Check.ThrowInvalidSchema("Invalid type field declaration.");
            else if (modifier == SchemaTypeModifier.Enum && tokens.Length != 2)
                Check.ThrowInvalidSchema("Invalid enum type field declaration.");

            string fieldName = tokens[0];
            if (!int.TryParse(tokens[modifier == null ? 2 : 1], out int fieldIndex))
                Check.ThrowInvalidSchema("Invalid field index.");

            ParserContext.Current.EnsureEmptyTypeFieldIndex(fieldIndex);

            if (modifier == SchemaTypeModifier.Enum)
                return new SchemaTypeField(fieldName, fieldIndex, null, null, false, null, ParserContext.Current.CurrentLine);

            bool isNullable = fieldName[^1] == '?';
            if (isNullable)
                fieldName = fieldName[0..^1];

            SchemaTypeFieldValueType fieldValueType = GetValueType(tokens[1]);

            IDictionary<string, object> metadata = metadataInput != null ? ReadMetadata(metadataInput) : null;

            return new SchemaTypeField(fieldName, fieldIndex, fieldValueType, rawDefaultValue, isNullable, metadata, ParserContext.Current.CurrentLine);
        }

        private static SchemaTypeFieldValueType GetValueType(string input)
        {
            string[] tokens = input.Trim().Replace(")", "").Split("(", StringSplitOptions.TrimEntries);
            string valueType = tokens[0];
            string elementType = tokens.Length == 2 ? tokens[1] : null;

            if (elementType == null)
                return GetPrimitiveOrCustomValueType(valueType);
            else
            {
                string[] elementTypeTokens = elementType.Split(',', StringSplitOptions.TrimEntries);

                if (valueType == SchemaFieldValueTypes.List)
                    if (elementTypeTokens.Length == 1)
                        return SchemaTypeFieldValueType.List(GetPrimitiveOrCustomValueType(elementType));
                    else
                        Check.ThrowInvalidSchema("List types can only specify element type, not key-value pairs.");
                else if (valueType == SchemaFieldValueTypes.Map)
                    if (elementTypeTokens.Length == 2)
                        return SchemaTypeFieldValueType.Map(GetPrimitiveOrCustomValueType(elementTypeTokens[0]), GetPrimitiveOrCustomValueType(elementTypeTokens[1]));
                    else
                        Check.ThrowInvalidSchema("Map types should specify key and value types.");
                else
                    Check.ThrowInvalidSchema("Only list and map values can have element types.");
            }

            return null;
        }

        private static SchemaTypeFieldValueType GetPrimitiveOrCustomValueType(string valueType)
        {
            if (SchemaFieldValueTypes.Values.Any(x => x == valueType))
                return SchemaTypeFieldValueType.Primitive(valueType);
            else
                return SchemaTypeFieldValueType.Custom(valueType);
        }

        private static (string typeName, SchemaTypeModifier? modifier) ReadTypeNameAndModifier(string input)
        {
            string[] parts = input.Split(" ", StringSplitOptions.TrimEntries).TakeWhile(x => x != Keywords.StartNewType).ToArray();
            if (parts.Length > 3)
                Check.ThrowInvalidSchema("Invalid type declaration.");

            string typeKeyword = parts[0];
            if (typeKeyword != Keywords.Type)
                Check.ThrowInvalidSchema($"Expected 'type' keyword, given '{typeKeyword}'");

            string typeName = parts[1];
            if (!mTypeNameRegex.IsMatch(typeName))
                Check.ThrowInvalidSchema($"Type name does not match expected regex: '^[a-zA-Z_]*$'");

            SchemaTypeModifier? modifier = null;
            if (parts.Length == 3)
                modifier = ParseModifier(parts[2]);

            return (typeName, modifier);
        }

        private static SchemaTypeModifier ParseModifier(string modifier)
            => modifier switch
            {
                Keywords.Enum => SchemaTypeModifier.Enum,
                _ => throw Check.InvalidSchema($"Invalid schema type modifier '{modifier}'")
            };

        private static IDictionary<string, object> ReadMetadata(string input)
        {
            Dictionary<string, object> metadata = new();
            var entries = input
                .Replace("(", "")
                .Replace(")", "")
                .Replace(Keywords.Metadata, string.Empty)
                .Split(",", StringSplitOptions.TrimEntries)
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

        private static object ParseMetadataValue(string input)
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

        private static int? ReadVersion(string input)
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
                return null;
            }
        }
    }
}