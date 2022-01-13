﻿using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Builder;
using SchemaInterpreter.Parser.Definition;
using System;
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
        private static readonly IEqualityComparer<SchemaPackage> mSchemaFileEqualityComparer = EqualityComparerFactory.Create<SchemaPackage>(
            (a, b) => a.Name == b.Name,
            x => x.Name.GetHashCode());

        private ICollection<SchemaPackage> mPackages; // The list of schema files that has been detected.
        private SchemaPackage mCurrentPackage;

        /// <summary>
        /// The current parser context, if any.
        /// </summary>
        public static ParserContext Current => mInstance;

        /// <summary>
        /// The current line of the file that is been reading.
        /// </summary>
        public FileLine CurrentLine { get; set; }

        /// <summary>
        /// If a type is being built, its builder class.
        /// </summary>
        public SchemaTypeBuilder CurrentTypeBuilder { get; set; }

        /// <summary>
        /// The current package being built.
        /// </summary>
        public SchemaPackage CurrentPackage
        {
            get => mCurrentPackage;
            set
            {
                mCurrentPackage = value;
                if (!mPackages.Any(x => x.Name == value.Name))
                    mPackages.Add(value);
            }
        }

        /// <summary>
        /// The list of files added to the current context.
        /// </summary>
        public IEnumerable<SchemaPackage> Packages => mPackages;

        private ParserContext()
        {
            mPackages = new HashSet<SchemaPackage>(mSchemaFileEqualityComparer);
        }

        /// <summary>
        /// Creates a new parser context instance
        /// </summary>
        public static ParserContext CreateContext()
        {
            mInstance = new();
            return mInstance;
        }

        /// <summary>
        /// Builds the current <see cref="SchemaTypeBuilder"/> and adds it to the current package.
        /// </summary>
        public void BuildCurrentTypeBuilder()
        {
            if(CurrentPackage == null)
                Check.ThrowInternal("No package is selected to add a schema type.");

            CurrentPackage.AddType(CurrentTypeBuilder.Build());
            CurrentTypeBuilder = null;
        }

        /// <summary>
        /// Checks if a type exists searching by its id. If not, an exception is thrown.
        /// </summary>
        public void EnsureTypeId(string typeId, string typeName, string package)
        {
            var enumerable = Packages;
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
            var enumerable = Packages;
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
        /// Ensures a field name is not in use.
        /// </summary>
        public void EnsureEmptyTypeFieldName(string name)
        {
            if (CurrentTypeBuilder.Fields.Any(x => x.Name == name))
                Check.ThrowInvalidSchema("Field name already defined.");
        }

        /// <summary>
        /// Parses and generates all the default values for all types.
        /// </summary>
        public void GenerateDefaultValues()
        {
            Logger.Debug("Generating default values.");

            foreach (SchemaTypeField field in mPackages.SelectMany(x => x.Types).SelectMany(x => x.Fields).Where(x => x.HasDefaultValue && !x.IsDefaultValueGenerated))
            {
                Logger.Debug($"Generating default value for field {field.Name}.");

                // Set the current line to the line where the field is declared
                CurrentLine = field.Line;

                try
                {
                    string rawDefaultValue = field.DefaultValue as string;
                    object value = ValueParser.Parse(rawDefaultValue, field.ValueType);
                    field.SetDefaultValue(value);
                }
                catch(Exception ex)
                {
                    if (ex is OverflowException)
                        Check.ThrowInvalidSchema($"Invalid default value for field ${field.Name}. It overflows the value type supported size.");
                    else 
                        throw;
                }
            }
        }

        /// <summary>
        /// Reruns a verification over all types to check if all of them exist.
        /// </summary>
        public void VerifyAllTypes()
        {
            Logger.Debug("Running type verification.");

            foreach (SchemaTypeField field in mPackages.SelectMany(x => x.Types).Where(x => x.Modifier == null).SelectMany(x => x.Fields).Where(x => !SchemaFieldValueTypes.Primitives.Contains(x.ValueType.TypeName)))
            {
                Logger.Debug($"Verifying field {field.Name}.");

                CurrentLine = field.Line;

                if(field.ValueType is ListSchemaFieldValueType listValue && listValue.ElementType is CustomSchemaFieldValueType elementType)
                {
                    Logger.Debug("Ensure list types.");
                    var (name, package) = SchemaTypeField.GetNameAndPackage(elementType.CustomType);
                    EnsureTypeId(SchemaTypeField.GetId(elementType.CustomType), name, package);
                }
                else if(field.ValueType is MapSchemaFieldValueType mapValue)
                {
                    Logger.Debug("Ensuring map types.");

                    if (mapValue.KeyType is CustomSchemaFieldValueType keyType)
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
                    Logger.Debug("Ensuring custom type.");

                    var (name, package) = SchemaTypeField.GetNameAndPackage(customType.CustomType);
                    EnsureTypeId(SchemaTypeField.GetId(customType.CustomType), name, package);
                }
            }
        }

        /// <summary>
        /// Verifies that all enums have consecutive, starting from 0, field index
        /// </summary>
        public void VerifyEnums()
        {
            Logger.Debug("Verifying enum types.");

            foreach (SchemaType type in mPackages.SelectMany(x => x.Types).Where(x => x.Modifier == SchemaTypeModifier.Enum))
            {
                if(type.Fields.First().Index != 0)
                    Check.ThrowInvalidSchema("First enum field should have 0-index.");

                if (!type.Fields.Select(x => x.Index).IsConsecutive())
                    Check.ThrowInvalidSchema("Enum field indexes should be consecutive.");
            }
        }

        /// <summary>
        /// Gets the compiled files and clear the current ParserContext
        /// </summary>
        public IEnumerable<SchemaPackage> GetCompiledAndClear()
        {
            Logger.Debug("Compiling and clearing...");

            var files = mPackages.ToList();
            Clear();
            return files;
        }

        /// <summary>
        /// Clears the current ParserContext
        /// </summary>
        public void Clear()
        {
            mInstance = null;
            mPackages = new HashSet<SchemaPackage>(mSchemaFileEqualityComparer);
            CurrentTypeBuilder = null;
            CurrentLine = null;
        }
    }
}