namespace SchemaInterpreter.Parser.Definition
{
    public abstract class SchemaTypeFieldValueType
    {
        private readonly string mName;

        /// <summary>
        /// The type of value the field holds.
        /// </summary>
        public string TypeName => mName;

        public SchemaTypeFieldValueType(string valueType)
        {
            mName = valueType;
        }

        public override string ToString() => TypeName;

        public static SchemaTypeFieldValueType Primitive(string typeName) => new PrimitiveSchemaFieldValueType(typeName);

        public static SchemaTypeFieldValueType List(string elementTypeName) => new ListSchemaFieldValueType(elementTypeName);

        public static SchemaTypeFieldValueType Map(string keyTypeName, string valueTypeName) => new MapSchemaFieldValueType(keyTypeName, valueTypeName);
    }

    public class PrimitiveSchemaFieldValueType : SchemaTypeFieldValueType
    {
        public PrimitiveSchemaFieldValueType(string valueType) : base(valueType) { }
    }

    public class ListSchemaFieldValueType : SchemaTypeFieldValueType
    {
        /// <summary>
        /// The type of element the list holds.
        /// </summary>
        public PrimitiveSchemaFieldValueType ElementType { get; }

        public ListSchemaFieldValueType(string elementType) : base(SchemaFieldValueTypes.List) 
        {
            ElementType = new PrimitiveSchemaFieldValueType(elementType);
        }
    }

    public class MapSchemaFieldValueType : SchemaTypeFieldValueType
    {
        /// <summary>
        /// The type of element the map key holds.
        /// </summary>
        public PrimitiveSchemaFieldValueType KeyType { get; }

        /// <summary>
        /// The type of element the map value holds.
        /// </summary>
        public PrimitiveSchemaFieldValueType ValueType { get; }

        public MapSchemaFieldValueType(string keyType, string valueType) : base(SchemaFieldValueTypes.Map)
        {
            KeyType = new PrimitiveSchemaFieldValueType(keyType);
            ValueType = new PrimitiveSchemaFieldValueType(valueType);
        }
    }
}