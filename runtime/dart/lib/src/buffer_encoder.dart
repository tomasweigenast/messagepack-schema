part of '../messagepack_schema.dart';

void _mergeBuffer(Uint8List buffer, SchemaFieldSet fieldSet) {
  var unpacker = Unpacker.fromList(buffer);
  for(var field in fieldSet.fields) {
    _unpackField(field, unpacker, fieldSet);
  }
}

Uint8List _encodeBuffer(SchemaFieldSet fieldSet) {
  var packer = Packer();

  for(var field in fieldSet.fields) {
    _packField(field, packer, fieldSet);
  }

  return packer.takeBytes();
}

void _unpackField(SchemaField field, Unpacker unpacker, SchemaFieldSet fieldSet) {
  dynamic value = _unpackValue(field.valueType, unpacker, field.customBuilder);
  _checkNullability(field, value);
  
  if(field.valueType is _ListFieldValueType) {
    List listValue = value as List;
    _checkNullability(field, value);

    for(dynamic listValue in listValue) {
      fieldSet.value(field.index).add(listValue);
    }

  } else if(field.valueType is _MapFieldValueType) {
    Map mapValue = value as Map;

      for(dynamic mapEntry in mapValue.entries) {
      fieldSet.value(field.index)[mapEntry.key] = mapEntry.value;
    }

  } else {
    fieldSet.setValue(field.index, value);
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

void _packField(SchemaField field, Packer packer, SchemaFieldSet fieldSet) {
  dynamic value = fieldSet.value(field.index);
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