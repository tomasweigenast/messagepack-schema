part of '../messagepack_schema.dart';

abstract class SchemaType<T extends Object> {
  /// Info about the current schema type. 
  SchemaTypeInfo<T> get info_;

  /// Returns the name of the type.
  String get name_ => info_.typeName;

  SchemaType();

  SchemaType.fromBuffer(Uint8List buffer) {
    mergeFromBuffer(buffer);
  }

  SchemaType.fromJson(Map<String, Object?> json) {
    mergeFromJson(json);
  }

  /// Writes the current type instance to a buffer.
  Uint8List toBuffer() {
    var packer = Packer();

    for(var field in info_.fieldSet.fields) {
      _packField(field, packer);
    }

    return packer.takeBytes();
  }

  /// Serializes the current type instance to json.
  Map<String, Object?> toJson() {
    return _toJson(info_.fieldSet);
  }
  
  /// Subclasses internal use.
  dynamic readValue_(int fieldIndex) {
    var field = info_.fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    return field.value;
  }

  /// Subclasses internal use.
  void setValue_(int fieldIndex, dynamic value) {
    var field = info_.fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    field.value = value;
  }

  /// Merges a encoded messagepack buffer to the current type instance.
  void mergeFromBuffer(Uint8List buffer) {
    _mergeBuffer(buffer);
  }

  /// Merges a encoded JSON map to the current type instance.
  void mergeFromJson(Map<String, Object?> map) {
    _mergeJson<T>(map, info_.fieldSet);
  }

  void _mergeBuffer(Uint8List buffer) {
    var unpacker = Unpacker.fromList(buffer);
    for(var field in info_.fieldSet.fields) {
      _unpackField(field, unpacker);
    }
  }

  dynamic _unpackField(SchemaField field, Unpacker unpacker) {
    dynamic value = _unpackValue(field.valueType, unpacker);
    _checkNullability(field, value);

    field.value = value;

    return value;
  }

  dynamic _unpackValue(SchemaFieldValueType valueType, Unpacker unpacker) {
    switch(valueType.typeCode) {
      case _SchemaFieldValueTypeCodes.stringType:
        return unpacker.unpackString();

      case _SchemaFieldValueTypeCodes.booleanType:
        return unpacker.unpackBool();

      case _SchemaFieldValueTypeCodes.intType:
        return unpacker.unpackInt();

      case _SchemaFieldValueTypeCodes.doubleType:
        return unpacker.unpackDouble();

      case _SchemaFieldValueTypeCodes.binaryType:
        return Uint8List.fromList(unpacker.unpackBinary());

      case _SchemaFieldValueTypeCodes.listType:
        int listLength = unpacker.unpackListLength();
        var outputList = [];
        for(int i = 0; i < listLength; i++) {
          outputList.add(_unpackValue((valueType as _ListFieldValueType).elementType, unpacker));
        }

        return outputList;

      case _SchemaFieldValueTypeCodes.mapType:
        int mapLength = unpacker.unpackMapLength();
        var mapType = valueType as _MapFieldValueType;
        return {for (var i = 0; i < mapLength; i++) _unpackValue(mapType.keyType, unpacker): _unpackValue(mapType.valueType, unpacker)};

      case _SchemaFieldValueTypeCodes.customType:
        return unpacker.unpackBinary();
    }
  }

  void _packField(SchemaField field, Packer packer) {
    _checkNullability(field, field.value);
    _packValue(field.value, field.valueType, packer);
  }

  void _packValue(dynamic value, SchemaFieldValueType valueType, Packer packer) {
    switch(valueType.typeCode) {
      case _SchemaFieldValueTypeCodes.stringType:
        packer.packString(value);
        break;

      case _SchemaFieldValueTypeCodes.booleanType:
        packer.packBool(value);
        break;

      case _SchemaFieldValueTypeCodes.intType:
        packer.packInt(value);
        break;

      case _SchemaFieldValueTypeCodes.doubleType:
        packer.packDouble(value);
        break;

      case _SchemaFieldValueTypeCodes.binaryType:
        packer.packBinary(value);
        break;

      case _SchemaFieldValueTypeCodes.listType:
        var list = value as List;
        var listType = valueType as _ListFieldValueType;
        packer.packListLength(list.length);
        for(var listValue in list) {
          _packValue(listValue, listType.elementType, packer);
        }
        break;

      case _SchemaFieldValueTypeCodes.mapType:
        var map = value as Map;
        var mapType = valueType as _MapFieldValueType;
        packer.packMapLength(map.length);
        for(var mapEntry in map.entries) {
          _packValue(mapEntry.key, mapType.keyType, packer);
          _packValue(mapEntry.value, mapType.valueType, packer);
        }
        break;

      case _SchemaFieldValueTypeCodes.customType:
        var customType = value as SchemaType;
        packer.packBinary(customType.toBuffer());
        break;
    }
  }

  void _checkNullability(SchemaField field, dynamic value){
    if(!field.isNullable && value == null) {
      throw NotNullError(fieldName: field.name, typeName: field.valueType.typeName);
    }
  }
}