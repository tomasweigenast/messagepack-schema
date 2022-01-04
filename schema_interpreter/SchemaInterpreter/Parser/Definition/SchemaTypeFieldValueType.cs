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

        public static SchemaTypeFieldValueType Custom(string typeName) => new CustomSchemaFieldValueType(typeName);

        public static SchemaTypeFieldValueType List(SchemaTypeFieldValueType elementType) => new ListSchemaFieldValueType(elementType);

        public static SchemaTypeFieldValueType Map(SchemaTypeFieldValueType keyType, SchemaTypeFieldValueType valueType) => new MapSchemaFieldValueType(keyType, valueType);
    }

    public class PrimitiveSchemaFieldValueType : SchemaTypeFieldValueType
    {
        public PrimitiveSchemaFieldValueType(string valueType) : base(valueType) { }
    }

    public class CustomSchemaFieldValueType : SchemaTypeFieldValueType
    {
        public string CustomType { get; }

        public CustomSchemaFieldValueType(string customType) : base(SchemaFieldValueTypes.Custom) 
        {
            CustomType = customType;
        }
    }

    public class ListSchemaFieldValueType : SchemaTypeFieldValueType
    {
        /// <summary>
        /// The type of element the list holds.
        /// </summary>
        public SchemaTypeFieldValueType ElementType { get; }

        public ListSchemaFieldValueType(SchemaTypeFieldValueType elementType) : base(SchemaFieldValueTypes.List) 
        {
            ElementType = elementType;
        }
    }

    public class MapSchemaFieldValueType : SchemaTypeFieldValueType
    {
        /// <summary>
        /// The type of element the map key holds.
        /// </summary>
        public SchemaTypeFieldValueType KeyType { get; }

        /// <summary>
        /// The type of element the map value holds.
        /// </summary>
        public SchemaTypeFieldValueType ValueType { get; }

        public MapSchemaFieldValueType(SchemaTypeFieldValueType keyType, SchemaTypeFieldValueType valueType) : base(SchemaFieldValueTypes.Map)
        {
            KeyType = keyType;
            ValueType = valueType;
        }
    }
}