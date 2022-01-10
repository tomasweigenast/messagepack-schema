part of '../messagepack_schema.dart';

Map<String, Object?> _toJson(SchemaFieldSet fieldSet) {
  Map<String, Object?> output = {};
  for(var field in fieldSet.fields) {
    output[field.name] = _writeValue(field.valueType, field.value);
  }

  return output;
}

Object _writeValue(SchemaFieldValueType valueType, dynamic value) {
  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.stringType:
    case _SchemaFieldValueTypeCodes.booleanType:
    case _SchemaFieldValueTypeCodes.intType:
      return value;

    case _SchemaFieldValueTypeCodes.doubleType:
      double doubleValue = value as double;
      if(doubleValue.isNaN) {
        return "NaN";
      }

      if(doubleValue.isInfinite) {
        throw DoubleNotInfiniteError();
      }

      return doubleValue;

    case _SchemaFieldValueTypeCodes.binaryType:
      return base64.encode(value as Uint8List);

    case _SchemaFieldValueTypeCodes.listType:
      var list = value as List;
      var listType = valueType as _ListFieldValueType;
      return list.map((e) => _writeValue(listType.elementType, e)).toList();

    case _SchemaFieldValueTypeCodes.mapType:
      var map = value as Map;
      var mapType = valueType as _MapFieldValueType;
      return map.map((key, value) => MapEntry(_writeValue(mapType.keyType, key), _writeValue(mapType.valueType, value)));

    case _SchemaFieldValueTypeCodes.customType:
      var customType = value as SchemaType;
      return customType.toJson();

    default: 
      throw InvalidTypeError(valueType);
  }
}

void _mergeJson(Map<String, Object?> map, SchemaFieldSet fieldSet) {
  for(var field in fieldSet.fields) {
    Object? value = _readValue(map[field.name], field.valueType);
    field.value = value ?? field.defaultValue;
  }
}

Object? _readValue(Object? value, SchemaFieldValueType valueType) {
  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.stringType:
      if(value is String) {
        return value;
      }

      throw InvalidValueError("Expected string value.");      

    case _SchemaFieldValueTypeCodes.booleanType:
      if(value is bool) {
        return value;
      }

      throw InvalidValueError("Expected bool value.");     

    case _SchemaFieldValueTypeCodes.intType:
      if(value is int) {
        return value;
      }

      throw InvalidValueError("Expected int value.");  

    case _SchemaFieldValueTypeCodes.doubleType:
      if(value is double) {
        return value;
      }

      throw InvalidValueError("Expected double value.");     

    case _SchemaFieldValueTypeCodes.binaryType:
      try {
        var buffer = base64.decode(value as String);
        return buffer;
      } catch(_) {
        throw InvalidValueError("Expected base64 encoded value.");   
      }

    case _SchemaFieldValueTypeCodes.listType:
      var list = value as List;
      var listType = valueType as _ListFieldValueType;
      return list.map((e) => _writeValue(listType.elementType, e)).toList();

    case _SchemaFieldValueTypeCodes.mapType:
      var map = value as Map;
      var mapType = valueType as _MapFieldValueType;
      return map.map((key, value) => MapEntry(_writeValue(mapType.keyType, key), _writeValue(mapType.valueType, value)));

    case _SchemaFieldValueTypeCodes.customType:
      var customType = value as SchemaType;
      return customType.toJson();

    default: 
      throw InvalidTypeError(valueType);
  }
}