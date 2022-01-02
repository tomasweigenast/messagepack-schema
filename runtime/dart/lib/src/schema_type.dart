part of '../messagepack_schema.dart';

class SchemaType {
  final String _name;
  final List<_SchemaField> _fields;

  const SchemaType(this._name, this._fields);

  Uint8List toBuffer() {
    var packer = Packer();

    for(var field in _fields) {
      // packer.packDouble(v)
    }

    return packer.takeBytes();
  }
}