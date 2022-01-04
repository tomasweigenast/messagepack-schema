using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser.Definition
{
    /// <summary>
    /// Contains information about a schema type field.
    /// </summary>
    public class SchemaTypeField
    {
        private readonly SchemaTypeFieldValueType mValueType;
        private readonly string mName;
        private readonly int mIndex;
        private readonly bool mIsNullable;
        private readonly IEnumerable<KeyValuePair<string, object>> mMetadata;
        private readonly SchemaLine mLine;

        private object mDefaultValue;
        private bool mIsDefaultValueGenerated;

        /// <summary>
        /// The value type to set.
        /// </summary>
        public SchemaTypeFieldValueType ValueType => mValueType;
        
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name => mName;

        /// <summary>
        /// The index of the field.
        /// </summary>
        public int Index => mIndex;
        
        /// <summary>
        /// A default value for the field.
        /// </summary>
        public object DefaultValue => mDefaultValue;

        /// <summary>
        /// Indicates if has a default value.
        /// </summary>
        public bool HasDefaultValue => mDefaultValue != null;

        /// <summary>
        /// Indicates if the default value was generated or not.
        /// </summary>
        public bool IsDefaultValueGenerated => mIsDefaultValueGenerated;

        /// <summary>
        /// A flag indicating if the field is nullable.
        /// </summary>
        public bool IsNullable => mIsNullable;

        /// <summary>
        /// A list of key-value pair of metadata information.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Metadata => mMetadata;

        /// <summary>
        /// The line where the field is defined.
        /// </summary>
        public SchemaLine Line => mLine;

        public SchemaTypeField(string name, int index, SchemaTypeFieldValueType valueType, object defaultValue, bool nullable, IEnumerable<KeyValuePair<string, object>> metadata, SchemaLine line)
        {
            mIndex = index;
            mValueType = valueType;
            mName = name;
            mDefaultValue = defaultValue;
            mIsNullable = nullable;
            mMetadata = metadata;
            mLine = line;
        }

        /// <summary>
        /// Sets the default value
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetDefaultValue(object value)
        {
            if (mIsDefaultValueGenerated)
                return;

            mDefaultValue = value;
            mIsDefaultValueGenerated = true;
        }

        public override string ToString() 
            => $"Field: {Name} - Index [{Index}] - Type [{ValueType}] - Default Value [{DefaultValue}] - Nullable [{IsNullable}] - Metadata [{Metadata.Select(x => $"{x.Key}:{x.Value}")}]";
    }
}