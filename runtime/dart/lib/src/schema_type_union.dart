part of '../messagepack_schema.dart';

abstract class SchemaTypeUnion<T> extends NamedEncodable {
  SchemaFieldSet<T> get fieldSet_;

  // The current value of the union.
  dynamic _currentValue;
  
  // The index of the field that is set.
  int? _setField;

  /// A flag that indicates if the union does not have a value.
  bool get isUnset => _currentValue == null;

  @mustCallSuper
  SchemaTypeUnion();

  void $setValue_(int fieldIndex, dynamic value) {
    _currentValue = value;
    _setField = fieldIndex;
  }

  dynamic $value_(int fieldIndex) {
    if(_setField == null) {
      throw UnionError("Union is not set.");
    }

    if(_setField != fieldIndex) {
      var field = fieldSet_[_setField!]!;
      throw UnionError("Union is set to field ${field.dartName} not to ${fieldSet_[fieldIndex]!.dartName}.");
    }

    return _currentValue;
  }

  int $whichField_() => _setField ?? -1;

  /// Clears the union value.
  void clearUnion() {
    _currentValue = null;
    _setField = null;
  }

  @override
  Uint8List toBuffer() {
    // As union encodes differently than normal types, _encodeBuffer() cannot be used
    var packer = Packer();

    if(_setField == null) {
      packer.packNull();
    } else {
      var valueType = fieldSet_[_setField!]!.valueType;
      // Pack the field set first
      packer.packInt(_setField!);

      // Pack the value now
      _packValue(_currentValue, valueType, packer);
    }

    return packer.takeBytes();
  }

  @override
  void mergeFromBuffer(Uint8List buffer) {
    var unpacker = Unpacker.fromList(buffer);
    int? setField = unpacker.unpackInt();
    
    // Clear the union if the field set is not present
    if(setField == null) {
      clearUnion();
    } else {
      // Unpack value
      var field = fieldSet_[_setField!]!;
      dynamic value = _unpackValue(field.valueType, unpacker, field.customBuilder);

      // Set union value
      $setValue_(setField, value);
    }
  }

  @override
  String fieldName() => _setField == null ? "" : fieldSet_[$whichField_()]!.dartName;

  @override
  Object? toJson() {
    if(_setField == null) {
      return null;
    }

    var field = fieldSet_[_setField!]!;
    return _writeValue(field.valueType, _currentValue);
  }

  @override
  void mergeFromJson(Object? map) {
    if(map is Map) {
      _mergeJson(map, fieldSet_);
      _setField = fieldSet_._values.indexWhere((element) => element != null);
      if(_setField != null) {
        _currentValue = fieldSet_._values[_setField!];
      }
    } else {
      throw StateError("invalid json as map.");
    }
  }
}