using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;

namespace SchemaInterpreter.Parser.Builder
{
    /// <summary>
    /// Provides methods to build a <see cref="Schemapackage"/>
    /// </summary>
    public class SchemaPackageBuilder
    {
        /// <summary>
        /// The name of the package
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of types
        /// </summary>
        public List<SchemaType> Types { get; set; }
    }
}