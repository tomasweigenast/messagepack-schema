part of '../messagepack_schema.dart';

/// Contains useful information about a [SchemaType]
class SchemaTypeInfo<T> {
  
  /// The name of the type
  final String typeName;

  /// The package where the type is in.
  final String package;

  /// The list of fields in the schema
  final SchemaFieldSet<T> fieldSet;

  String get fullName => "$package.$typeName";

  SchemaTypeInfo({required this.typeName, required this.package, required this.fieldSet});
}