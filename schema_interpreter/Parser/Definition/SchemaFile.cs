using System.Collections.Generic;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Contains information about a schema file.
    /// </summary>
    public class SchemaFile
    {
        private readonly int mVersion;
        private readonly IList<SchemaType> mTypes;

        /// <summary>
        /// The version of the schema.
        /// </summary>
        public int Version => mVersion;

        /// <summary>
        /// The list of types in the schema file.
        /// </summary>
        public IList<SchemaType> Types => mTypes;

        public SchemaFile(int version, IList<SchemaType> types)
        {
            mVersion = version;
            mTypes = types;
        }
    }
}