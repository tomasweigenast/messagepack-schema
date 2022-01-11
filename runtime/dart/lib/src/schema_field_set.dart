part of '../messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final Map<int, SchemaField> _fields;
  final List _values;

  SchemaFieldSet(Map<int, SchemaField> fields) 
    : _fields = SplayTreeMap.from(fields),
      _values = List.filled(fields.length, null, growable: false);

  /// Retrieves information about a field at [i].
  SchemaField? operator [](int i) => _fields[i];
  
  /// Retrieves the value of a field at [i]
  dynamic value(int i) => _values[i];
  
  /// Sets the value of [i] field to [value].
  void setValue(int i, dynamic value) {
    _values[i] = value;
  }

  /// Returns an iterable which contains the list of fields, sorted by index.
  Iterable<SchemaField> get fields => _fields.entries.orderBy((element) => element.key).map((e) => e.value);

  SchemaFieldSet<T> clone() {
    return SchemaFieldSet<T>(_fields);
  }
}