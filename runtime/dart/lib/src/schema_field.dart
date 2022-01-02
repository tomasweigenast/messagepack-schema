part of '../messagepack_schema.dart';

class _SchemaField<T> {
  final String name;
  final int index;
  final T? defaultValue;
  
  T? value;

  _SchemaField({required this.name, required this.index, required this.defaultValue});
}