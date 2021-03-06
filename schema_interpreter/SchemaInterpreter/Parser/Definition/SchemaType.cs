using SchemaInterpreter.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Contains information about a type in the schema.
    /// </summary>
    public class SchemaType
    {
        private readonly string mName;
        private readonly string mPackage;
        private readonly SchemaTypeModifier? mModifier;
        private readonly IEnumerable<SchemaTypeField> mFields;

        /// <summary>
        /// The id of the schema type based on the hashcode of the name.
        /// </summary>
        public string Id => CommonHelpers.CalculateMD5(mName);

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name => mName;

        /// <summary>
        /// The package of the type. Its the name of the file.
        /// </summary>
        public string Package => mPackage;

        /// <summary>
        /// The list of fields in the type.
        /// </summary>
        public IList<SchemaTypeField> Fields => mFields.OrderBy(x => x.Index).ToList().AsReadOnly();

        /// <summary>
        /// The modifier of the type.
        /// </summary>
        public SchemaTypeModifier? Modifier => mModifier;
        
        /// <summary>
        /// The full name of the type.
        /// </summary>
        public string FullName => $"{Package}.{Name}";

        public SchemaType(string name, string package, SchemaTypeModifier? modifier, IEnumerable<SchemaTypeField> fields)
        {
            mPackage = package;
            mName = name;
            mModifier = modifier;
            mFields = fields;
        }

        public override string ToString() => $"Type: {Name} - Modifier [{Modifier}] - Fields [{Fields.Count()}]";
    }

    public enum SchemaTypeModifier
    {
        Enum,
        Union
    }

    public static class SchemaTypeModifierExtensions
    {
        public static SchemaTypeModifier ToSchemaTypeModifier(this string modifier)
            => modifier switch
            {
                Keywords.Enum => SchemaTypeModifier.Enum,
                Keywords.Union => SchemaTypeModifier.Union,
                _ => throw Check.InvalidSchema($"Invalid schema type modifier '{modifier}'")
            };
    }
}