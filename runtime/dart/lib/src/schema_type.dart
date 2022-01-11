part of '../messagepack_schema.dart';

abstract class SchemaType<T extends Object> {
  /// Info about the current schema type. 
  SchemaTypeInfo<T> get info_;

  /// The list of fields
  late final SchemaFieldSet<T> _fieldSet;

  /// Returns the name of the type.
  String get name_ => info_.typeName;

  @mustCallSuper
  SchemaType() {
    _fieldSet = info_.fieldSet.clone();
    _ensureNotNulls();
  }

  SchemaType.fromBuffer(Uint8List buffer) {
    mergeFromBuffer(buffer);
  }

  SchemaType.fromJson(Map<String, Object?> json) {
    mergeFromJson(json);
  }

  /// Writes the current type instance to a buffer.
  Uint8List toBuffer() {
    var packer = Packer();

    for(var field in _fieldSet.fields) {
      _packField(field, packer);
    }

    return packer.takeBytes();
  }

  /// Serializes the current type instance to json.
  Map<String, Object?> toJson() {
    return _toJson(_fieldSet);
  }
  
  /// Subclasses internal use.
  dynamic readValue_(int fieldIndex) {
    var field = _fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    return _fieldSet.value(fieldIndex);
  }

  void _ensureNotNulls() {
    for(var field in _fieldSet._fields.values.where((element) => !element.isNullable && element.defaultValue != null)) {
      _fieldSet.setValue(field.index, field.defaultValue);
    }
  }

  /// Subclasses internal use.
  void setValue_(int fieldIndex, dynamic value) {
    var field = _fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    _fieldSet.setValue(fieldIndex, value);
  }

  /// Merges a encoded messagepack buffer to the current type instance.
  void mergeFromBuffer(Uint8List buffer) {
    _mergeBuffer(buffer);
  }

  /// Merges a encoded JSON map to the current type instance.
  void mergeFromJson(Map<String, Object?> map) {
    _mergeJson<T>(map, _fieldSet);
  }

  void _mergeBuffer(Uint8List buffer) {
    var unpacker = Unpacker.fromList(buffer);
    for(var field in _fieldSet.fields) {
      _unpackField(field, unpacker);
    }
  }

  void _unpackField(SchemaField field, Unpacker unpacker) {
    dynamic value = _unpackValue(field.valueType, unpacker, field.customBuilder);
    _checkNullability(field, value);
    
    if(field.valueType is _ListFieldValueType) {
      List listValue = value as List;
      _checkNullability(field, value);

      for(dynamic listValue in listValue) {
        _fieldSet.value(field.index).add(listValue);
      }

    } else if(field.valueType is _MapFieldValueType) {
      Map mapValue = value as Map;

       for(dynamic mapEntry in mapValue.entries) {
        _fieldSet.value(field.index)[mapEntry.key] = mapEntry.value;
      }

    } else {
      _fieldSet.setValue(field.index, value);
    }
  }

  dynamic _unpackValue(SchemaFieldValueType valueType, Unpacker unpacker, CustomBuilder? builder) {
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
        return unpacker.unpackBinary();

      case _SchemaFieldValueTypeCodes.listType:
        int listLength = unpacker.unpackListLength();
        var outputList = [];
        for(int i = 0; i < listLength; i++) {
          outputList.add(_unpackValue((valueType as _ListFieldValueType).elementType, unpacker, null));
        }

        return outputList;

      case _SchemaFieldValueTypeCodes.mapType:
        int mapLength = unpacker.unpackMapLength();
        var mapType = valueType as _MapFieldValueType;
        return {for (var i = 0; i < mapLength; i++) _unpackValue(mapType.keyType, unpacker, null): _unpackValue(mapType.valueType, unpacker, null)};

      case _SchemaFieldValueTypeCodes.enumType:
        int? enumValue = unpacker.unpackInt();
        if(enumValue == null) {
          return null;
        }
        
        return builder!([enumValue]);

      case _SchemaFieldValueTypeCodes.customType:
        SchemaType type = builder!([]);
        return type..mergeFromBuffer(unpacker.unpackBinary());
    }
  }

  void _packField(SchemaField field, Packer packer) {
    dynamic value = _fieldSet.value(field.index);
    _checkNullability(field, value);
    _packValue(value, field.valueType, packer);
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

      case _SchemaFieldValueTypeCodes.enumType:
        if(value == null) {
          packer.packNull();
        } else {
          var enumerator = value as SchemaTypeEnum;
          packer.packInt(enumerator.index);
        }
        break;

      case _SchemaFieldValueTypeCodes.customType:
        if(value == null){
          packer.packNull();
        } else {
          var customType = value as SchemaType;
          packer.packBinary(customType.toBuffer());
        }
        break;
    }
  }

  void _checkNullability(SchemaField field, dynamic value){
    if(!field.isNullable && value == null) {
      throw NotNullError(fieldName: field.name, typeName: field.valueType.typeName);
    }
  }
}