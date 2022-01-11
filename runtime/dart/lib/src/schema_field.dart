part of '../messagepack_schema.dart';

typedef CustomBuilder = SchemaField Function();

class SchemaField<T> {
  final String name;
  final String dartName;
  final int index;
  final T defaultValue;
  final SchemaFieldValueType valueType;
  final bool isNullable;
  final CustomBuilder? customBuilder;

  T? value;

  SchemaField._internal({
    required this.name, 
    required this.dartName,
    required this.index, 
    required this.valueType, 
    required this.isNullable, 
    required this.defaultValue, 
    required this.customBuilder});

  factory SchemaField(String fieldName, String dartName, int index, SchemaFieldValueType valueType, bool isNullable, T? defaultValue, CustomBuilder? customBuilder) {
    return SchemaField<T>._internal(
      name: fieldName,
      dartName: dartName,
      index: index,
      valueType: valueType,
      defaultValue: defaultValue ?? _findDefaultValue<T>(),
      isNullable: isNullable,
      customBuilder: customBuilder,
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
      customBuilder: customBuilder
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
      customBuilder: customBuilder
    );
  }
}

T _findDefaultValue<T>() {
  switch(T) {
    case num:
    case int: 
      return 0 as T;
    case double: 
      return 0 as T;

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