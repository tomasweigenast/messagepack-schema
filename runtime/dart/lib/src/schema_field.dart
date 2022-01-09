part of '../messagepack_schema.dart';

class SchemaField<T> {
  final String name;
  final int index;
  final T? defaultValue;
  final SchemaFieldValueType valueType;

  T? value;

  SchemaField({required this.name, required this.index, required this.valueType, required this.defaultValue});
}

enum SchemaFieldValueType {
  string,
  int,
  double,
  boolean,
  list,
  map,
  binary
}