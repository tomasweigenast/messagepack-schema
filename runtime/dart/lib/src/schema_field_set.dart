part of '../messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final SplayTreeMap<int, SchemaField> _fields;
  final SplayTreeMap<String, SchemaField> _fieldsByName;
  final SplayTreeMap<int, List<String>> _unions;
  final List _values;

  List<SchemaField>? _fieldsByIndex;

  SchemaFieldSet(SchemaFieldSetBuilder builder) 
    : _fields = builder._fields,
      _fieldsByName = builder._fieldsByName,
      _unions = builder._unions,
      _values = List.filled(builder._fields.length, null, growable: false);

  SchemaFieldSet._fromOther(this._fields, this._fieldsByName, this._unions) : _values = List.filled(_fields.length, null, growable: false);

  /// Retrieves information about a field at [i].
  SchemaField? operator [](int i) => _fields[i];
  
  /// Retrieves information about a field searching by its name.
  SchemaField? byName(String name) => _fieldsByName[name];

  /// Retrieves the value of a field at [i]
  dynamic value(int i) => _values[i];
  
  bool get hasUnions => _unions.isNotEmpty;
  Iterable<SchemaField> get fieldsByIndex => _fieldsByIndex ?? _getFieldsByIndex(); 

  /// Sets the value of [i] field to [value].
  void setValue(int i, dynamic value) {
    _values[i] = value;
  }

  /// Returns an iterable which contains the list of fields, sorted by index.
  Iterable<SchemaField> get fields => _fields.entries.orderBy((element) => element.key).map((e) => e.value);

  SchemaFieldSet<T> clone() {
    return SchemaFieldSet<T>._fromOther(_fields, _fieldsByName, _unions);
  }

  /// Calculates the hashcode of the type.
  int _calculateHashCode() {

    // Hashes a field.
    int _hashSchemaField(int previousHash, SchemaField field, value) {
      
      // Empty list does not modify the hash
      if(value is List && value.isEmpty) {
        return previousHash;
      }

      previousHash = _Hashing.combine(previousHash, field.index);
      switch(field.valueType.typeCode) {
        case _SchemaFieldValueTypeCodes.listType:
          previousHash = _Hashing.combine(previousHash, _Hashing.hashObjects(value));
          break;

        case _SchemaFieldValueTypeCodes.mapType:
          var map = value as Map;
          previousHash = _Hashing.combine(previousHash, _Hashing.hashObjects(map.keys));
          previousHash = _Hashing.combine(previousHash, _Hashing.hashObjects(map.values));
          break;

        case _SchemaFieldValueTypeCodes.binaryType:
          previousHash = _Hashing.combine(previousHash, _Hashing.hashObjects(value));
          break;

        case _SchemaFieldValueTypeCodes.enumType:
          var enumValue = value as SchemaTypeEnum;
          previousHash = _Hashing.combine(previousHash, enumValue.index);
          break;

        default:
          previousHash = _Hashing.combine(previousHash, value.hashCode);
          break;

      }

      return previousHash;
    }
  
    // Hash fields
    var hash = _Hashing.combine(0, fieldsByIndex.where((element) => _values[element.index] != null).fold(0, (int hash, SchemaField field) => _hashSchemaField(hash, field, _values[field.index])));
    return hash;
  }

  /// Calculates if two [SchemaFieldSet] are equals.
  bool _calculateEquality(SchemaFieldSet other) {
    for(var i = 0; i < _values.length; i++) {
      if(!_equalValues(_values[i], other._values[i])) {
        return false;
      }
    }

    return true;
  }

  bool _equalValues(left, right) {
    // Null values compare to same
    if(left == null && right == null) {
      return true;
    }

    if(left != null && right != null) {
      return _Equality.deepEquality(left, right);
    }

    var value = left ?? right;
    if((value is List || value is Map) && value.isEmpty) {
      return true;
    }

    return false;
  }

  List<SchemaField> _getFieldsByIndex() {
    _fieldsByIndex = _fields.entries.orderBy((element) => element.key).map((e) => e.value).toList();
    return _fieldsByIndex!;
  }
}

class SchemaFieldSetBuilder<T> {
  final SplayTreeMap<int, SchemaField> _fields = SplayTreeMap();
  final SplayTreeMap<String, SchemaField> _fieldsByName = SplayTreeMap();
  final SplayTreeMap<int, List<String>> _unions = SplayTreeMap();

  SchemaFieldSetBuilder<T> addField<TField>(SchemaField<TField> field) {
    _fields[field.index] = field;
    _fieldsByName[field.dartName] = field;

    if(field.isUnion) {
      _unions[field.index] = field.unionFields!;
    }
    return this;
  }
}