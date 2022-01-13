using SchemaInterpreter.Helpers;
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
        private List<SchemaType> mTypes;

        /// <summary>
        /// The version of the schema file.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// The list of types in the schema file.
        /// </summary>
        public IList<SchemaType> Types => mTypes.OrderBy(x => x.FullName).ToList();

        /// <summary>
        /// The path of the directory where the package resides in.
        /// For example: utils/common.mpack -> utils is the directory.
        /// Another example: utils/shared/user.mpack -> utils/shared will be the directory.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// The name of the package.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The id of the package.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The list of package names that are imported in this package.
        /// </summary>
        public HashSet<string> Imports { get; set; }

        public SchemaPackage(string packageName, int version)
        {
            if (packageName.Contains('/'))
            {
                string[] tokens = packageName.Split('/');
                packageName = tokens.Last();
                Directory = string.Join('/', tokens.SkipLast(1));
            }

            Name = packageName;
            Version = version;
            Imports = new();
            mTypes = new();
            Id = Directory.IsNullOrWhitespace() ? CommonHelpers.CalculateMD5(Name) : CommonHelpers.CalculateMD5($"{Directory}.{Name}");
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