using System.Linq;

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

        public override string ToString()
        {
            if (this is ListSchemaFieldValueType listType)
                return $"list({listType.ElementType.TypeName})";
            else if (this is MapSchemaFieldValueType mapType)
                return $"map({mapType.KeyType.TypeName},{mapType.ValueType.TypeName})";
            else if (this is CustomSchemaFieldValueType custom)
                return custom.CustomType;
            else
                return TypeName;
        }

        public static SchemaTypeFieldValueType Primitive(string typeName)
            => new PrimitiveSchemaFieldValueType(typeName, SchemaFieldValueTypes.TypeCodes[typeName]);

        public static SchemaTypeFieldValueType Custom(string typeName) => new CustomSchemaFieldValueType(typeName);

        public static SchemaTypeFieldValueType List(SchemaTypeFieldValueType elementType) => new ListSchemaFieldValueType(elementType);

        public static SchemaTypeFieldValueType Map(SchemaTypeFieldValueType keyType, SchemaTypeFieldValueType valueType) => new MapSchemaFieldValueType(keyType, valueType);

        public static SchemaTypeFieldValueType Parse(string input)
        {
            if (input.StartsWith("list"))
            {
                string elementType = input.Remove(0, 1).Remove(input.Length - 1);
                return List(Parse(elementType));
            }
            else if (input.StartsWith("map"))
            {
                string[] elements = input.Remove(0, 1).Remove(input.Length - 1).Split(',');
                return Map(Parse(elements[0]), Parse(elements[1]));
            }
            else if (SchemaFieldValueTypes.Primitives.Any(x => x == input))
                return Primitive(input);
            else
                return Custom(input);
        }

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