using System.Collections.Generic;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Contains information about a type in the schema.
    /// </summary>
    public class SchemaType
    {
        private readonly string mName;
        private readonly IEnumerable<SchemaTypeField> mFields;

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name => mName;

        /// <summary>
        /// The list of fields in the type.
        /// </summary>
        public IEnumerable<SchemaTypeField> Fields => mFields;

        public SchemaType(string name, IEnumerable<SchemaTypeField> fields)
        {
            mName = name;
            mFields = fields;
        }
    }
}