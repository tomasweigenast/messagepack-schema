import 'package:messagepack_schema/messagepack_schema.dart';

class InvalidTypeError extends Error {
  final SchemaFieldValueType valueType;

  InvalidTypeError(this.valueType);

  @override
  String toString() => "Value type ${valueType.typeName} cannot be serialized.";
}