part of '../messagepack_schema.dart';

class SchemaTypeEnum {
  /// The enum index, an integer value.
  final int index;

  /// The name of the value.
  final String name;

  /// Creates a new enum.
  const SchemaTypeEnum(this.index, this.name);

  // Implemented by subclasses. 
  @override
  bool operator ==(Object other);

  @override
  int get hashCode => index;

  /// Returns the enum's name.
  @override
  String toString() => name;
}