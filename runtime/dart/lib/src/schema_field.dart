part of '../messagepack_schema.dart';

typedef CustomBuilder = dynamic Function(List args);
typedef NamedField<T> = String Function(T);

class SchemaField<T> {
  final String name;
  final String dartName;
  final int index;
  final T defaultValue;
  final SchemaFieldValueType valueType;
  final bool isNullable;
  final CustomBuilder? customBuilder;
  final NamedField<T>? nameFunction;
  final bool skipIfNull;
  final List<String>? unionFields;

  bool get isUnion => valueType.typeCode == _SchemaFieldValueTypeCodes.unionType;

  SchemaField._internal({
    required this.name, 
    required this.dartName,
    required this.index, 
    required this.valueType, 
    required this.isNullable, 
    required this.defaultValue, 
    required this.customBuilder,
    required this.nameFunction,
    required this.skipIfNull,
    required this.unionFields});

  factory SchemaField(String fieldName, String dartName, int index, SchemaFieldValueType valueType, bool isNullable, T? defaultValue, CustomBuilder? customBuilder) {
    return SchemaField<T>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: valueType,
      defaultValue: defaultValue ?? _findDefaultValue<T>(),
      isNullable: isNullable,
      customBuilder: customBuilder,
      nameFunction: null,
      skipIfNull: false,
      unionFields: null
    );
  }

  static SchemaField<List<T>> list<T>(String fieldName, String dartName, int index, SchemaFieldValueType elementType, CustomBuilder? customBuilder) {
    return SchemaField<List<T>>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: SchemaFieldValueType.list(elementType),
      defaultValue: <T>[],
      isNullable: false,
      customBuilder: customBuilder,
      nameFunction: null,
      skipIfNull: false,
      unionFields: null
    );
  }

  static SchemaField<Map<TKey, TValue>> map<TKey, TValue>(String fieldName, String dartName, int index, SchemaFieldValueType keyType, SchemaFieldValueType valueType, CustomBuilder? customBuilder) {
    return SchemaField<Map<TKey, TValue>>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: SchemaFieldValueType.map(keyType, valueType),
      defaultValue: <TKey, TValue>{},
      isNullable: false,
      customBuilder: customBuilder,
      nameFunction: null,
      skipIfNull: false,
      unionFields: null
    );
  }

  static SchemaField<TEnum> enumerator<TEnum extends SchemaTypeEnum>(String fieldName, String dartName, int index, bool isNullable, TEnum defaultValue, TEnum? Function(int index) customBuilder) {
    return SchemaField<TEnum>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: SchemaFieldValueType.enumerator,
      defaultValue: defaultValue,
      isNullable: isNullable,
      customBuilder: (args) => customBuilder(args[0] as int),
      nameFunction: null,
      skipIfNull: true,
      unionFields: null
    );
  }

  static SchemaField<TType> type<TType extends SchemaType>(String fieldName, String dartName, String typeName, int index, bool isNullable, TType Function() customBuilder) {
    return SchemaField<TType>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: SchemaFieldValueType.custom(typeName),
      isNullable: isNullable,
      defaultValue: customBuilder(),
      customBuilder: (_) => customBuilder(),
      nameFunction: null,
      skipIfNull: false,
      unionFields: null
    );
  }

  static SchemaField<TType> union<TType extends SchemaTypeUnion>(String fieldName, String dartName, String typeName, int index, List<String> fieldNames, TType Function() customBuilder, NamedField<TType> nameFunction) {
    return SchemaField<TType>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: SchemaFieldValueType.union,
      isNullable: true,
      defaultValue: customBuilder(),
      customBuilder: (_) => customBuilder(),
      nameFunction: nameFunction,
      skipIfNull: true,
      unionFields: fieldNames
    );
  }
}

T _findDefaultValue<T>() {
  switch(T) {
    case num:
    case int: 
      return 0 as T;
    case double: 
      return 0.0 as T;

    case bool:
      return false as T;

    case String:
      return "" as T;

    case Uint8List:
      return Uint8List.fromList(const <int>[]) as T;

    default:
      throw StateError("Cant find default value for type $T");
  }
}