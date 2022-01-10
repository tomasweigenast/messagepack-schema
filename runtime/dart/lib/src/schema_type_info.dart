part of '../messagepack_schema.dart';

/// Contains useful information about a [SchemaType]
class SchemaTypeInfo<T> {
  
  /// The name of the type
  late final String typeName;

  /// The package where the type is in.
  late final String package;

  /// The list of fields in the schema
  final SchemaFieldSet<T> fieldSet;

  String get fullName => "$package.$typeName";

  SchemaTypeInfo({required String fullName, required this.fieldSet}) {
    var parts = fullName.split(".");
    package = parts[0];
    typeName = parts[1];
  }
}