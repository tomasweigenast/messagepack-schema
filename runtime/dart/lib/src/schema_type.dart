part of '../messagepack_schema.dart';

abstract class SchemaType<T extends Object> extends Encodable {
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

  @override
  Uint8List toBuffer() {
    return _encodeBuffer(_fieldSet);
  }

  @override
  Object? toJson() {
    return _toJson(_fieldSet);
  }
  
  /// Subclasses internal use.
  dynamic $readValue_(int fieldIndex) {
    var field = _fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    return _fieldSet.value(fieldIndex);
  }

  /// Subclasses internal use.
  void $setValue_(int fieldIndex, dynamic value) {
    var field = _fieldSet[fieldIndex];
    if(field == null) {
      throw UnknownTypeField(fieldIndex);
    }

    _fieldSet.setValue(fieldIndex, value);
  }

  @override
  void mergeFromBuffer(Uint8List buffer) {
    _mergeBuffer(buffer, _fieldSet);
  }

  @override
  void mergeFromJson(Object? map) {
    _mergeJson<T>(map, _fieldSet);
  }

  void _ensureNotNulls() {
    for(var field in _fieldSet._fields.values.where((element) => !element.isNullable && element.defaultValue != null)) {
      _fieldSet.setValue(field.index, field.defaultValue);
    }
  }

  @override
  int get hashCode => _fieldSet._calculateHashCode();

  @override
  bool operator ==(other) {
    if(identical(this, other)) {
      return true;
    }

    if(other is SchemaType) {
      return _fieldSet._calculateEquality(other._fieldSet);
    }

    return false;
  } 

}