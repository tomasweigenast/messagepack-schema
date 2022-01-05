using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Contains information about a schema file.
    /// </summary>
    public class SchemaFile
    {
        private IList<SchemaType> mTypes;

        /// <summary>
        /// The version of the schema.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// The list of types in the schema file.
        /// </summary>
        public IList<SchemaType> Types => mTypes.OrderBy(x => x.FullName).ToList();

        /// <summary>
        /// The name of the file, a.k.a. package name.
        /// </summary>
        public string Name { get; }

        public SchemaFile(string packageName, int version)
        {
            Name = packageName;
            Version = version;
            mTypes = new List<SchemaType>();
        }

        /// <summary>
        /// Adds a new type
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void AddType(SchemaType type)
        {
            mTypes.Add(type);
        }
    }
}