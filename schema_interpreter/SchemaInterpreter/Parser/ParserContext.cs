using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// The current parsing context which contains references between
    /// schemas and types.
    /// </summary>
    public class ParserContext
    {
        private static ParserContext mInstance;
        private static readonly IEqualityComparer<SchemaFile> mSchemaFileEqualityComparer = EqualityComparerFactory.Create<SchemaFile>(
            (a, b) => a.Name == b.Name,
            x => x.Name.GetHashCode());

        private ICollection<SchemaFile> mFiles; // The list of schema files that has been detected.

        /// <summary>
        /// The current parser context, if any.
        /// </summary>
        public static ParserContext Current => mInstance;

        /// <summary>
        /// The current line of the file that is been reading.
        /// </summary>
        public FileLine CurrentLine { get; set; }

        /// <summary>
        /// The current type builder
        /// </summary>
        public SchemaTypeBuilder CurrentTypeBuilder { get; set; }

        /// <summary>
        /// The list of files added to the current context.
        /// </summary>
        public IEnumerable<SchemaFile> Files => mFiles;

        private ParserContext()
        {
            mFiles = new HashSet<SchemaFile>(mSchemaFileEqualityComparer);
        }

        /// <summary>
        /// Creates a new parser context instance
        /// </summary>
        public static void Create()
        {
            mInstance = new();
        }

        /// <summary>
        /// Finds a type by its id.
        /// </summary>
        /// <param name="id">The type id.</param>
        public SchemaType FindType(string id) 
            => mFiles.SelectMany(x => x.Types).FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Finds a type by its full name.
        /// </summary>
        /// <param name="nane">The full name of the type.</param>
        public SchemaType FindTypeByFullName(string fullName) 
            => mFiles.SelectMany(x => x.Types).FirstOrDefault(x => x.FullName == fullName);
        
        /// <summary>
        /// Adds a new schema type to a package.
        /// </summary>
        public void Add(SchemaType type, string package)
        {
            SchemaFile file = mFiles.FirstOrDefault(x => x.Name == package);
            if (file == null)
                Check.ThrowInternal($"Schema package '{package}' not found.");

            file.Types.Add(type);
        }

        /// <summary>
        /// Adds a new schema package.
        /// </summary>
        public void AddPackage(SchemaFile file)
        {
            mFiles.Add(file);
        }

        /// <summary>
        /// Ensures a type name is not already in use.
        /// </summary>
        public void EnsureEmptyTypeName(string name)
        {
            if (Files.SelectMany(x => x.Types).Any(x => x.Name.ToLower() == name.ToLower()))
                Check.ThrowInvalidSchema($"Type name {name} is already declared.");
        }

        /// <summary>
        /// Checks if a type exists searching by its id. If not, an exception is thrown.
        /// </summary>
        public void EnsureTypeId(string typeId, string typeName, string package)
        {
            var enumerable = Files;
            if (package != null)
                enumerable = enumerable.Where(x => x.Name == package);
            else
                enumerable = enumerable.Take(1);

            if (!enumerable.SelectMany(x => x.Types).Any(x => x.Id == typeId))
                Check.ThrowInvalidSchema($"Type name {typeName} was not found.");
        }

        /// <summary>
        /// Checks if a value exists inside a value searching by its declaration name. If not, an exception is thrown.
        /// </summary>
        public void EnsureTypeValue(string value, string typeId, string typeName, string package)
        {
            var enumerable = Files;
            if (package != null)
                enumerable = enumerable.Where(x => x.Name == package);
            else
                enumerable = enumerable.Take(1);

            if (!enumerable.SelectMany(x => x.Types).Where(x => x.Id == typeId).SelectMany(x => x.Fields).Any(x => x.Name == value))
                Check.ThrowInvalidSchema($"Value {value} is not present in type {typeName}.");
        }

        /// <summary>
        /// Ensures an index is not already in use by a field.
        /// </summary>
        public void EnsureEmptyTypeFieldIndex(int index)
        {
            if (CurrentTypeBuilder.ExistsIndex(index))
                Check.ThrowInvalidSchema("Type field index already defined.");
        }

        /// <summary>
        /// Parses and generates all the default values for all types.
        /// </summary>
        public void GenerateDefaultValues()
        {
            foreach (SchemaTypeField field in mFiles.SelectMany(x => x.Types).SelectMany(x => x.Fields).Where(x => x.HasDefaultValue && !x.IsDefaultValueGenerated))
            {
                // Set the current line to the line where the field is declared
                CurrentLine = field.Line;

                string rawDefaultValue = field.DefaultValue as string;
                object value = ValueParser.Parse(rawDefaultValue, field.ValueType);
                field.SetDefaultValue(value);
            }
        }

        /// <summary>
        /// Reruns a verification over all types to check if all of them exist.
        /// </summary>
        public void VerifyAllTypes()
        {
            foreach (SchemaTypeField field in mFiles.SelectMany(x => x.Types).Where(x => x.Modifier == null).SelectMany(x => x.Fields).Where(x => !SchemaFieldValueTypes.Primitives.Contains(x.ValueType.TypeName)))
            {
                CurrentLine = field.Line;

                if(field.ValueType is ListSchemaFieldValueType listValue && listValue.ElementType is CustomSchemaFieldValueType elementType)
                {
                    var (name, package) = SchemaTypeField.GetNameAndPackage(elementType.CustomType);
                    EnsureTypeId(SchemaTypeField.GetId(elementType.CustomType), name, package);
                }
                else if(field.ValueType is MapSchemaFieldValueType mapValue)
                {
                    if(mapValue.KeyType is CustomSchemaFieldValueType keyType)
                    {
                        var (name, package) = SchemaTypeField.GetNameAndPackage(keyType.CustomType);
                        EnsureTypeId(SchemaTypeField.GetId(keyType.CustomType), name, package);
                    }

                    if(mapValue.ValueType is CustomSchemaFieldValueType valueType)
                    {
                        var (name, package) = SchemaTypeField.GetNameAndPackage(valueType.CustomType);
                        EnsureTypeId(SchemaTypeField.GetId(valueType.CustomType), name, package);
                    }
                }
                else if(field.ValueType is CustomSchemaFieldValueType customType)
                {
                    var (name, package) = SchemaTypeField.GetNameAndPackage(customType.CustomType);
                    EnsureTypeId(SchemaTypeField.GetId(customType.CustomType), name, package);
                }
            }
        }

        /// <summary>
        /// Gets the compiled files and clear the current ParserContext
        /// </summary>
        public IEnumerable<SchemaFile> GetCompiledAndClear()
        {
            // Run last verification first.
            VerifyAllTypes();

            var files = mFiles.ToList();
            Clear();
            return files;
        }

        /// <summary>
        /// Clears the current ParserContext
        /// </summary>
        public void Clear()
        {
            mInstance = null;
            mFiles = new HashSet<SchemaFile>(mSchemaFileEqualityComparer);
            CurrentTypeBuilder = null;
            CurrentLine = null;
        }
    }
}