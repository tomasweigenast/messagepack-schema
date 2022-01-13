namespace SchemaInterpreter.Parser.Definition
{
    public abstract class SchemaTypeFieldValueType
    {
        private readonly string mName;
        private readonly int mTypeCode;

        /// <summary>
        /// The type of value the field holds.
        /// </summary>
        public string TypeName => mName;

        /// <summary>
        /// The typecode of the type.
        /// </summary>
        public int TypeCode => mTypeCode;

        public SchemaTypeFieldValueType(string valueType, int typeCode)
        {
            mName = valueType;
            mTypeCode = typeCode;
        }

        public override string ToString() => TypeName;

        public static SchemaTypeFieldValueType Primitive(string typeName)
            => new PrimitiveSchemaFieldValueType(typeName, SchemaFieldValueTypes.TypeCodes[typeName]);

        public static SchemaTypeFieldValueType Custom(string typeName) => new CustomSchemaFieldValueType(typeName);

        public static SchemaTypeFieldValueType List(SchemaTypeFieldValueType elementType) => new ListSchemaFieldValueType(elementType);

        public static SchemaTypeFieldValueType Map(SchemaTypeFieldValueType keyType, SchemaTypeFieldValueType valueType) => new MapSchemaFieldValueType(keyType, valueType);

        public override bool Equals(object obj)
        {
            if (obj is not SchemaTypeFieldValueType other)
                return false;

            return other.TypeCode == TypeCode;
        }

        public override int GetHashCode() => TypeCode;
    }

    public class PrimitiveSchemaFieldValueType : SchemaTypeFieldValueType
    {
        public PrimitiveSchemaFieldValueType(string valueType, int typeCode) : base(valueType, typeCode) { }
    }

    public class CustomSchemaFieldValueType : SchemaTypeFieldValueType
    {
        public string CustomType { get; }

        public CustomSchemaFieldValueType(string customType) : base(SchemaFieldValueTypes.Custom, SchemaFieldValueTypes.TypeCodes[SchemaFieldValueTypes.Custom]) 
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

        public ListSchemaFieldValueType(SchemaTypeFieldValueType elementType) : base(SchemaFieldValueTypes.List, SchemaFieldValueTypes.TypeCodes[SchemaFieldValueTypes.List]) 
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

        public MapSchemaFieldValueType(SchemaTypeFieldValueType keyType, SchemaTypeFieldValueType valueType) : base(SchemaFieldValueTypes.Map, SchemaFieldValueTypes.TypeCodes[SchemaFieldValueTypes.Map])
        {
            KeyType = keyType;
            ValueType = valueType;
        }
    }
}