part of '../messagepack_schema.dart';

/// Contains information about the value type of a schema type field.
abstract class SchemaFieldValueType {
  final String typeName;
  final int typeCode;

  const SchemaFieldValueType({required this.typeName, required this.typeCode});

  static const SchemaFieldValueType string = _PrimitiveFieldValueType(typeName: _SchemaFieldValueTypeNames.stringType, typeCode: _SchemaFieldValueTypeCodes.stringType);
  static const SchemaFieldValueType int64 = _PrimitiveFieldValueType(typeName: _SchemaFieldValueTypeNames.intType, typeCode: _SchemaFieldValueTypeCodes.intType);
  static const SchemaFieldValueType float64 = _PrimitiveFieldValueType(typeName: _SchemaFieldValueTypeNames.doubleType, typeCode: _SchemaFieldValueTypeCodes.doubleType);
  static const SchemaFieldValueType boolean = _PrimitiveFieldValueType(typeName: _SchemaFieldValueTypeNames.booleanType, typeCode: _SchemaFieldValueTypeCodes.booleanType);
  static const SchemaFieldValueType binary = _PrimitiveFieldValueType(typeName: _SchemaFieldValueTypeNames.binaryType, typeCode: _SchemaFieldValueTypeCodes.binaryType);
  factory SchemaFieldValueType.custom(String customTypeName) => _CustomFieldValueType(customTypeName: customTypeName);
  factory SchemaFieldValueType.list(SchemaFieldValueType elementType) => _ListFieldValueType(elementType: elementType);
  factory SchemaFieldValueType.map(SchemaFieldValueType keyType, SchemaFieldValueType valueType) => _MapFieldValueType(keyType: keyType, valueType: valueType);
}

class _PrimitiveFieldValueType extends SchemaFieldValueType {
  const _PrimitiveFieldValueType({required String typeName, required int typeCode}) : super(typeName: typeName, typeCode: typeCode);
}

class _ListFieldValueType extends SchemaFieldValueType {
  final SchemaFieldValueType elementType;
  
  const _ListFieldValueType({required this.elementType}) : super(typeName: _SchemaFieldValueTypeNames.listType, typeCode: _SchemaFieldValueTypeCodes.listType);
}

class _MapFieldValueType extends SchemaFieldValueType {
  final SchemaFieldValueType keyType;
  final SchemaFieldValueType valueType;

  const _MapFieldValueType({required this.keyType, required this.valueType}) : super(typeName: _SchemaFieldValueTypeNames.mapType, typeCode: _SchemaFieldValueTypeCodes.mapType);
}

class _CustomFieldValueType extends SchemaFieldValueType {
  final String customTypeName;

  const _CustomFieldValueType({required this.customTypeName}) : super(typeName: _SchemaFieldValueTypeNames.customType, typeCode: _SchemaFieldValueTypeCodes.customType);
}

class _SchemaFieldValueTypeCodes {
  const _SchemaFieldValueTypeCodes._();

  static const int stringType = 1;
  static const int intType = 9;
  static const int doubleType = 11;
  static const int booleanType = 12;
  static const int binaryType = 13;
  static const int listType = 14;
  static const int mapType = 15;
  static const int customType = 16;
}

class _SchemaFieldValueTypeNames {
  const _SchemaFieldValueTypeNames._();

  static const String stringType = "string";
  static const String intType = "int64";
  static const String doubleType = "float64";
  static const String booleanType = "boolean";
  static const String binaryType = "binary";
  static const String listType = "list";
  static const String mapType = "map";
  static const String customType = "custom";
}