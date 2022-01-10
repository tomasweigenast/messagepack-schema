part of '../messagepack_schema.dart';

typedef CustomBuilder<T> = T Function();

class SchemaField<T> {
  final String name;
  final String dartName;
  final int index;
  final T? defaultValue;
  final SchemaFieldValueType valueType;
  final bool isNullable;
  final CustomBuilder<T>? customBuilder;
  final String id;

  T? value;

  SchemaField({
    required this.name, 
    required this.dartName,
    required this.index, 
    required this.valueType, 
    required this.isNullable, 
    required this.defaultValue, 
    required this.customBuilder}) : id = Random().nextInt(656262626).toString();
}