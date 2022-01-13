using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Contains information about a schema package, a.k.a. file.
    /// </summary>
    public class SchemaPackage
    {
        // The list of types declared in the package.
        private IList<SchemaType> mTypes;

        /// <summary>
        /// The version of the schema file.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// The list of types in the schema file.
        /// </summary>
        public IList<SchemaType> Types => mTypes.OrderBy(x => x.FullName).ToList();

        /// <summary>
        /// The name of the package.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of package names that are imported in this package.
        /// </summary>
        public List<string> Imports { get; set; }

        public SchemaPackage(string packageName, int version, IEnumerable<string> imports)
        {
            Name = packageName;
            Version = version;
            mTypes = new List<SchemaType>();

            if(imports != null)
                Imports = new List<string>(imports);
        }

        /// <summary>
        /// Adds a type to the package.
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void AddType(SchemaType type)
        {
            mTypes.Add(type);
        }
    }
}