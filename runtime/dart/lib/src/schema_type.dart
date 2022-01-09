part of '../messagepack_schema.dart';

class SchemaType<T extends Object> {
  final String _name;
  final SchemaFieldSet<T> _fieldSet;

  const SchemaType(this._name, this._fieldSet);

  Uint8List toBuffer() {
    var packer = Packer();

    for(var field in _fieldSet.fields) {
      _packField(field, packer);
    }

    return packer.takeBytes();
  }

  void _packField(SchemaField field, Packer packer) {
    switch(field.valueType) {
      case SchemaFieldValueType.string:
        packer.packString(field.value);
        break;

      case SchemaFieldValueType.int:
        packer.packInt(field.value);
        break;

      case SchemaFieldValueType.double:
        packer.packDouble(field.value);
        break;

      case SchemaFieldValueType.boolean:
        packer.packBool(field.value);
        break;

      case SchemaFieldValueType.binary:
        packer.packBinary(field.value);
        break;

      case SchemaFieldValueType.list:
        var list = field.value as List;
        packer.packListLength(list.length);
        for(var value in list) {
          _packField(field, packer)
        }
        break;
    }
  }
}