using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchemaInterpreter.Parser.V1
{
    public class SchemaFileParser : ISchemaFileParser
    {
        private static readonly int mCurrentVersion = 1;
        private static readonly Regex mTypeNameRegex = new("^[a-zA-Z_]*$", RegexOptions.Compiled);

        private static readonly IList<SchemaType> mTypes = new List<SchemaType>();
        private static SchemaTypeBuilder mCurrentTypeBuilder = null;

        public int Version => 1;

        public async Task<SchemaFile> ParseFile(StreamReader reader)
        {
            mTypes.Clear();

            string line;
            int lineIndex = 0;
            bool commentStarted = false;
            int? version = null;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineIndex++;
                line = line.Replace("\t", "").Replace("\r", "").Trim();

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
                        throw new InvalidSchemaException("Multiline comments should start with a slash: '/'", lineIndex, line);

                // Start interpreting
                else
                {
                    if (line.StartsWith("version:"))
                    {
                        version = ReadVersion(line, lineIndex);
                        continue;
                    }
                    else
                    {
                        if (version == null)
                            throw new InvalidSchemaException("Schema does not specify a version. It must be the first line to be set.", lineIndex, line);
                        else
                        {
                            char lastLineChar = line[^1];
                            switch (lastLineChar)
                            {
                                // Starts a new type
                                case '{':
                                    if (mCurrentTypeBuilder != null)
                                        throw new InvalidSchemaException("When creating a type, closing tags should be present before creating a new one.", lineIndex, line);

                                    var (typeName, modifier) = ReadTypeNameAndModifier(line, lineIndex);
                                    CheckTypeName(typeName, line, lineIndex);
                                    mCurrentTypeBuilder = new SchemaTypeBuilder(typeName, modifier);
                                    break;

                                // Ends a type.
                                case '}':
                                    if (mCurrentTypeBuilder == null)
                                        throw new InvalidSchemaException("Before closing a type, a new one must be created.", lineIndex, line);

                                    mTypes.Add(mCurrentTypeBuilder.Build());
                                    mCurrentTypeBuilder = null;
                                    break;

                                default:
                                    if (mCurrentTypeBuilder == null)
                                        throw new InvalidSchemaException("Before specifying a field, a type must be created.", lineIndex, line);

                                    var typeField = ReadTypeField(line, lineIndex);
                                    mCurrentTypeBuilder.AddField(typeField);
                                    break;
                            }
                        }
                    }
                }

            }

            return new SchemaFile(1, mTypes);
        }

        private static SchemaTypeField ReadTypeField(string line, int lineIndex)
        {
            var modifier = mCurrentTypeBuilder.Modifier;
            string[] splitChars = mCurrentTypeBuilder.Modifier == null ? new string[] { ":", " ", "=", "@" } : new string[] { " " };
            string[] tokens = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            if (modifier == null && tokens.Length < 3)
                throw new InvalidSchemaException("Invalid type field declaration.", lineIndex, line);
            else if (modifier == SchemaTypeModifier.Enum && tokens.Length != 2)
                throw new InvalidSchemaException("Invalid enum type field declaration.", lineIndex, line);

            string fieldName = tokens[0];
            if (!int.TryParse(tokens[modifier == null ? 2 : 1], out int fieldIndex))
                throw new InvalidSchemaException("Invalid field index.", lineIndex, line);
            CheckTypeFieldIndex(fieldIndex, line, lineIndex);

            if (modifier == SchemaTypeModifier.Enum)
                return new SchemaTypeField(fieldName, fieldIndex, null, null, false, null);

            bool isNullable = fieldName[^1] == '?';
            if (isNullable)
                fieldName = fieldName[0..^2];

            SchemaTypeFieldValueType fieldValueType = GetValueType(tokens[1], lineIndex, line);

            object defaultValue = tokens.Length == 4 ? ValueParser.Parse(tokens[3], fieldValueType, lineIndex, line) : null;
            IDictionary<string, object> metadata = tokens.Length == 5 ? ReadMetadata(tokens[4], lineIndex, line) : null;

            return new SchemaTypeField(fieldName, fieldIndex, fieldValueType, defaultValue, isNullable, metadata);
        }

        private static SchemaTypeFieldValueType GetValueType(string input, int lineIndex, string line)
        {
            string[] tokens = input.Trim().Replace(")", "").Split("(", StringSplitOptions.TrimEntries);
            string valueType = tokens[0];
            string elementType = tokens.Length == 2 ? tokens[1] : null;

            if (elementType == null)
                return SchemaTypeFieldValueType.Primitive(valueType);
            else
            {
                string[] elementTypeTokens = elementType.Split(',', StringSplitOptions.TrimEntries);

                if (valueType == SchemaFieldValueTypes.List)
                    if (elementTypeTokens.Length == 1)
                        return SchemaTypeFieldValueType.List(elementType);
                    else
                        throw new InvalidSchemaException("List types can only specify element type, not key-value pairs.", lineIndex, line);
                else if (valueType == SchemaFieldValueTypes.Map)
                    if (elementTypeTokens.Length == 2)
                        return SchemaTypeFieldValueType.Map(elementTypeTokens[0], elementTypeTokens[1]);
                    else
                        throw new InvalidSchemaException("Map types should specify key and value types.", lineIndex, line);
                else
                    throw new InvalidSchemaException("Only list and map values can have element types.", lineIndex, line);
            }
        }

        private static (string typeName, SchemaTypeModifier? modifier) ReadTypeNameAndModifier(string line, int lineIndex)
        {
            string[] parts = line.Split(" ", StringSplitOptions.TrimEntries).TakeWhile(x => x != Keywords.StartNewType).ToArray();
            if (parts.Length > 3)
                throw new InvalidSchemaException("Invalid type declaration.", lineIndex, line);

            string typeKeyword = parts[0];
            if (typeKeyword != Keywords.Type)
                throw new InvalidSchemaException($"Expected 'type' keyword, given '{typeKeyword}'", lineIndex, line);

            string typeName = parts[1];
            if (!mTypeNameRegex.IsMatch(typeName))
                throw new InvalidSchemaException($"Type name does not match expected regex: '^[a-zA-Z_]*$'", lineIndex, line);

            SchemaTypeModifier? modifier = null;
            if (parts.Length == 3)
                modifier = ParseModifier(parts[2], line, lineIndex);

            return (typeName, modifier);
        }

        private static SchemaTypeModifier ParseModifier(string modifier, string line, int lineIndex)
            => modifier switch
            {
                Keywords.Enum => SchemaTypeModifier.Enum,
                _ => throw new InvalidSchemaException($"Invalid schema type modifier '{modifier}'", lineIndex, line)
            };

        private static IDictionary<string, object> ReadMetadata(string input, int lineIndex, string line)
        {
            Dictionary<string, object> metadata = new();
            var entries = input.Replace("(", "").Replace(")", "").Split(",", StringSplitOptions.TrimEntries).Select(x => x.Replace("]", "").Replace("[", "").Trim());
            foreach (string entry in entries)
            {
                string[] tokens = entry.Split(":");
                if (tokens.Length != 2)
                    throw new InvalidSchemaException("Metadata entries should contain exactly two values.", lineIndex, line);

                string key = tokens[0];
                if (key.StartsWith('"') && key.EndsWith('"'))
                    key = key.Replace('"', '\0');
                else
                    throw new InvalidSchemaException("Metadata keys can be only of type string.", lineIndex, line);

                object value = ParseMetadataValue(tokens[1], lineIndex, line);

                metadata[key] = value;
            }

            return metadata;
        }

        private static object ParseMetadataValue(string input, int lineIndex, string line)
        {
            if (input.Contains('"'))
                return input.Replace('"', '\0');
            else if (bool.TryParse(input, out bool boolValue))
                return boolValue;
            else if (long.TryParse(input, out long longValue))
                return longValue;
            else if (double.TryParse(input, out double doubleValue))
                return doubleValue;
            else
                throw new InvalidSchemaException("Metadata values can be only of string, boolean, int or float types.", lineIndex, line);
        }

        private static int? ReadVersion(string line, int lineIndex)
        {
            try
            {
                int version = int.Parse(line.Split("version:")[1]);
                if (version != mCurrentVersion)
                    throw new InvalidSchemaException($"Invalid version. Expected: 1 - Given: {version}", lineIndex, line);

                return version;
            }
            catch
            {
                throw new InvalidSchemaException("Invalid version.", lineIndex, line);
            }
        }

        private static void CheckTypeFieldIndex(int index, string line, int lineIndex)
        {
            if (mCurrentTypeBuilder.ExistsIndex(index))
                throw new InvalidSchemaException("Type field index already defined.", lineIndex, line);
        }

        private static void CheckTypeName(string name, string line, int lineIndex)
        {
            if (mTypes.Any(x => x.Name.ToLower() == name.ToLower()))
                throw new InvalidSchemaException($"Type name {name} is already declared.", lineIndex, line);
        }
    }
}