part of '../messagepack_schema.dart';

class SchemaField<T> {
  final String name;
  final int index;
  final T? defaultValue;
  final SchemaFieldValueType valueType;
  final bool isNullable;

  T? value;

  SchemaField({required this.name, required this.index, required this.valueType, required this.isNullable, required this.defaultValue});
}