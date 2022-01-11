part of '../messagepack_schema.dart';

Map<String, Object?> _toJson(SchemaFieldSet fieldSet) {
  Map<String, Object?> output = {};
  for(var field in fieldSet.fields) {
    Object value = _writeValue(field.valueType, field.value);
    output[field.dartName] = value;
  }

  return output;
}

Object _writeValue(SchemaFieldValueType valueType, dynamic value) {
  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.booleanType:
      return value ? true : false;

    case _SchemaFieldValueTypeCodes.stringType:
      return value as String;

    case _SchemaFieldValueTypeCodes.intType:
      return value as int;

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
      var newMap = <Object, Object>{};
      for(var entry in map.entries) {
        newMap[_convertToMapKey(mapType.keyType, entry.key)] = _writeValue(mapType.valueType, entry.value);
      }

      return newMap;

    case _SchemaFieldValueTypeCodes.customType:
      var customType = value as SchemaType;
      return customType.toJson();

    default: 
      throw InvalidTypeError(valueType);
  }
}

String _convertToMapKey(SchemaFieldValueType valueType, Object value) {
  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.stringType:
      return value as String;

    case _SchemaFieldValueTypeCodes.booleanType:
    case _SchemaFieldValueTypeCodes.doubleType:
    case _SchemaFieldValueTypeCodes.intType:
      return value.toString();

    default: 
      throw StateError("Value $value cannot be used as a map key.");
  }
}

void _mergeJson<T>(Map<String, Object?> map, SchemaFieldSet<T> fieldSet) {
  for(var field in fieldSet.fields) {
    Object? value = _readValue(map[field.dartName], field.valueType, field.isNullable, field.customBuilder);
    if(value is Iterable) {
      field.value = _getList(field.valueType as _ListFieldValueType);
      print(value.runtimeType);
      print(field.value.runtimeType);
      (field.value as List).addAll(value);
    } else {
      field.value = value ?? field.defaultValue;
    }
  }
}

Object? _readValue(Object? value, SchemaFieldValueType valueType, bool nullable, CustomBuilder? builder) {
  if(value == null) {
    return null;
  }

  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.stringType:
      if(value is String) {
        return value;
      }

      throw InvalidValueError("Expected string value. Given: ${value.runtimeType}");      

    case _SchemaFieldValueTypeCodes.booleanType:
      if(value is bool) {
        return value;
      }

      throw InvalidValueError("Expected bool value. Given: ${value.runtimeType}");     

    case _SchemaFieldValueTypeCodes.intType:
      if(value is int) {
        return value.toInt();
      } else if(value is num) {
        return value.toInt();
      }

      throw InvalidValueError("Expected int value. Given: ${value.runtimeType}");  

    case _SchemaFieldValueTypeCodes.doubleType:
      if(value is double) {
        return value.toDouble();
      } else if(value is num) {
        return value.toDouble();
      }

      throw InvalidValueError("Expected double value. Given: ${value.runtimeType}");     

    case _SchemaFieldValueTypeCodes.binaryType:
      try {
        var buffer = base64.decode(value as String);
        return buffer;
      } catch(_) {
        throw InvalidValueError("Expected base64 encoded value. Given: ${value.runtimeType}");   
      }

    case _SchemaFieldValueTypeCodes.listType:
      if(value is Iterable) {
        var listType = valueType as _ListFieldValueType;
        return value.map((e) => _readValue(e, listType.elementType, false, builder)).toList();
      }

      throw InvalidValueError("Expected iterable value. Given: ${value.runtimeType}");

    case _SchemaFieldValueTypeCodes.mapType:
      if(value is Map) {
        var mapType = valueType as _MapFieldValueType;
        return value.map((key, value) => MapEntry(_convertMapKey(mapType.keyType, key), _readValue(value, mapType.valueType, false, builder)));
      }

      throw InvalidValueError("Expected map value. Given: ${value.runtimeType}");

    case _SchemaFieldValueTypeCodes.customType:
      if(value is Map && builder != null) {
        return builder();
      }

      throw InvalidValueError("Expected custom type as map value. Given: ${value.runtimeType}");

    default: 
      throw InvalidTypeError(valueType);
  }
}

Object _convertMapKey(SchemaFieldValueType valueType, String key) {
  switch(valueType.typeCode) {
    case _SchemaFieldValueTypeCodes.stringType:
      return key;

    case _SchemaFieldValueTypeCodes.booleanType:
      key = key.toLowerCase();
      switch(key) {
        case 'true': return true;
        case 'false': return false;
        default: throw StateError("Invalid bool as string value: $key");
      }

    case _SchemaFieldValueTypeCodes.doubleType:
      return double.parse(key);

    case _SchemaFieldValueTypeCodes.intType:
      return int.parse(key);

    default: 
      throw StateError("Value $key cannot be used as a map key.");
  }
}

dynamic _getList(_ListFieldValueType listType) {
  switch(listType.elementType.typeCode) {
    case _SchemaFieldValueTypeCodes.booleanType:
      return <bool>[];

    case _SchemaFieldValueTypeCodes.stringType:
      return <String>[];

    case _SchemaFieldValueTypeCodes.intType:
      return <int>[];

    case _SchemaFieldValueTypeCodes.doubleType:
      return <double>[];

    case _SchemaFieldValueTypeCodes.binaryType:
      return <Uint8List>[];

    case _SchemaFieldValueTypeCodes.listType:
    case _SchemaFieldValueTypeCodes.mapType:
      throw StateError("Nested lists and maps are not supported.");

    case _SchemaFieldValueTypeCodes.customType:
      return <SchemaType>[];
  }
}