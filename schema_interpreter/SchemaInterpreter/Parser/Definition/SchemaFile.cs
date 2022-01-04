using System.Collections.Generic;

namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Contains information about a schema file.
    /// </summary>
    public class SchemaFile
    {
        /// <summary>
        /// The version of the schema.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// The list of types in the schema file.
        /// </summary>
        public IList<SchemaType> Types { get; }

        /// <summary>
        /// The name of the file, a.k.a. package name.
        /// </summary>
        public string Name { get; }

        public SchemaFile(string packageName, int version)
        {
            Name = packageName;
            Version = version;
            Types = new List<SchemaType>();
        }
    }
}