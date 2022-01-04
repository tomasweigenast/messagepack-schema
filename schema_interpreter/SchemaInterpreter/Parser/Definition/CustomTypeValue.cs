namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Represents a value of a custom type. It is useful when defining enum fields and default values.
    /// </summary>
    public class CustomTypeValue
    {
        /// <summary>
        /// The id of the type.
        /// </summary>
        public string TypeId { get; }

        /// <summary>
        /// The value type as a string.
        /// </summary>
        public string Value { get; }

        public CustomTypeValue(string typeId, string value)
        {
            TypeId = typeId;
            Value = value;
        }
    }
}