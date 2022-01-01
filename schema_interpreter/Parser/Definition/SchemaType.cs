namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Contains information about a type in the schema.
    /// </summary>
    public class SchemaType
    {
        private readonly string mName;

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name => mName;

        public SchemaType(string name)
        {
            mName = name;
        }
    }
}