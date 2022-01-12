part of '../messagepack_schema.dart';

Map<String, Object?> _toJson(SchemaFieldSet fieldSet) {
  Map<String, Object?> output = {};
  for(var field in fieldSet.fields) {
    var fieldValue = fieldSet.value(field.index);
    Object? value = _writeValue(field.valueType, fieldValue);
    
    // Skip null values
    if(value == null && field.skipIfNull) {
      continue;
    } 

    String fieldName = (field as dynamic).nameFunction?.call(fieldValue as dynamic) ?? field.dartName;
    output[fieldName] = value;
  }

  return output;
}

Object? _writeValue(SchemaFieldValueType valueType, dynamic value) {
  if(value == null) {
    return null;
  }

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
      var newMap = <Object, Object?>{};
      for(var entry in map.entries) {
        newMap[_convertToMapKey(mapType.keyType, entry.key)] = _writeValue(mapType.valueType, entry.value);
      }

      return newMap;

    case _SchemaFieldValueTypeCodes.enumType:
      return (value as SchemaTypeEnum).index;

    case _SchemaFieldValueTypeCodes.unionType:
      return (value as SchemaTypeUnion).toJson();

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

void _mergeJson<T>(Object? map, SchemaFieldSet<T> fieldSet) {
  // if the map is null, do nothing.
  if(map == null) {
    return;
  }

  var unknownFields = {};

  if(map is Map) {

    map.forEach((key, Object? mapValue) { 
      var field = fieldSet.byName(key);
      if(field == null) {
        unknownFields[key] = mapValue;
        return;
      }

      if(mapValue is List) {
        for(int i = 0; i < mapValue.length; i++) {
          var value = _readValue(mapValue[i], (field.valueType as _ListFieldValueType).elementType, false, field.customBuilder);
          fieldSet.value(field.index).add(value);
        }
      } else if(mapValue is Map) {
        switch(field.valueType.typeCode) {
          case _SchemaFieldValueTypeCodes.mapType:
            var mapFieldType = field.valueType as _MapFieldValueType;
            for(var entry in mapValue.entries) {
              var key = _readMapKey(entry.key, mapFieldType.keyType);
              var value = _readValue(entry.value, mapFieldType.valueType, false, null);

              fieldSet.value(field.index)[key] = value;
            }
            break;

          case _SchemaFieldValueTypeCodes.customType:
            SchemaType typeInstance = field.customBuilder!([]) as SchemaType;
            typeInstance.mergeFromJson(mapValue.cast<String, Object>());
            fieldSet.setValue(field.index, typeInstance);
            break;
        }
        
      } else {
        switch(field.valueType.typeCode) {
          case _SchemaFieldValueTypeCodes.unionType:
            SchemaTypeUnion unionInstance = field.customBuilder!([]) as SchemaTypeUnion;
            unionInstance.mergeFromJson(mapValue);

            fieldSet.setValue(field.index, unionInstance);
            break;

          default:
            dynamic value = _readValue(mapValue, field.valueType, field.isNullable, field.customBuilder);
            if(value == null && !field.isNullable) {
              value = field.defaultValue;
            }

            fieldSet.setValue(field.index, value);
            break;
        }
      }
    });
  } else {
    throw StateError("expected json object");
  }
}

dynamic _readValue(Object? value, SchemaFieldValueType valueType, bool nullable, CustomBuilder? builder) {
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

    case _SchemaFieldValueTypeCodes.enumType:
      if(value is int) {
        return builder!([value]);
      } 

      throw InvalidValueError("Expect enum value as int. Given: ${value.runtimeType}");

    case _SchemaFieldValueTypeCodes.customType:
      if(value is Map && builder != null) {
        return builder([]);
      }

      throw InvalidValueError("Expected custom type as map value. Given: ${value.runtimeType}");

    default: 
      throw InvalidTypeError(valueType);
  }
}

dynamic _readMapKey(Object? key, SchemaFieldValueType keyType) {
  switch(keyType.typeCode) {
    case _SchemaFieldValueTypeCodes.booleanType:
      switch(key) {
        case 'true': return true;
        case 'false': return false;
        default: throw StateError("Invalid boolean value. Given: $key");
      }

    case _SchemaFieldValueTypeCodes.stringType:
      return key;

    case _SchemaFieldValueTypeCodes.intType:
      try {
        return int.parse(key as String);
      } catch(_) {
        throw StateError("Invalid int as string value. Given: $key");
      }

    case _SchemaFieldValueTypeCodes.doubleType:
      try {
        return double.parse(key as String);
      } catch(_) {
        throw StateError("Invalid double as string value. Given: $key");
      }

    default: throw StateError("Not a valid key type: $keyType");
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