using System.Collections.Generic;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Contains information about a schema type field.
    /// </summary>
    public class SchemaTypeField
    {
        private readonly SchemaTypeFieldValueType mValueType;
        private readonly string mName;
        private readonly object mDefaultValue;
        private readonly bool mIsNullable;
        private readonly bool mIsOptional;
        private readonly IEnumerable<KeyValuePair<string, string>> mMetadata;

        /// <summary>
        /// The value type to set.
        /// </summary>
        public SchemaTypeFieldValueType ValueType => mValueType;
        
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name => mName;
        
        /// <summary>
        /// A default value for the field.
        /// </summary>
        public object DefaultValue => mDefaultValue;
        
        /// <summary>
        /// A flag indicating if the field is nullable.
        /// </summary>
        public bool IsNullable => mIsNullable;

        /// <summary>
        /// A flag indicating if the field is optional.
        /// The difference between this and <see cref="IsNullable"/>, is that the current one
        /// does not accept null values but does not throws an exception when the message is built and 
        /// the value is not set.
        /// </summary>
        public bool IsOptional => mIsOptional;

        /// <summary>
        /// A list of key-value pair of metadata information.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Metadata => mMetadata;

        public SchemaTypeField(string name, SchemaTypeFieldValueType valueType, object defaultValue, bool nullable, bool optional, IEnumerable<KeyValuePair<string, string>> metadata)
        {
            mValueType = valueType;
            mName = name;
            mDefaultValue = defaultValue;
            mIsNullable = nullable;
            mIsOptional = optional;
            mMetadata = metadata;
        }
    }
}