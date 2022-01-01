using System.Collections.Generic;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Contains information about a schema file.
    /// </summary>
    public class SchemaDefinition
    {
        private readonly IList<SchemaType> mTypes;

        public SchemaDefinition(IList<SchemaType> types)
        {
            mTypes = types;
        }
    }
}