part of '../messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final Map<int, SchemaField> _fields;
  final Map<String, SchemaField> _fieldsByName;
  final List _values;

  SchemaFieldSet(SchemaFieldSetBuilder builder) 
    : _fields = builder._fields,
      _fieldsByName = builder._fieldsByName,
      _values = List.filled(builder._fields.length, null, growable: false);

  SchemaFieldSet._fromOther(this._fields, this._fieldsByName) : _values = List.filled(_fields.length, null, growable: false);

  /// Retrieves information about a field at [i].
  SchemaField? operator [](int i) => _fields[i];
  
  /// Retrieves information about a field searching by its name.
  SchemaField? byName(String name) => _fieldsByName[name];

  /// Retrieves the value of a field at [i]
  dynamic value(int i) => _values[i];
  
  /// Sets the value of [i] field to [value].
  void setValue(int i, dynamic value) {
    _values[i] = value;
  }

  /// Returns an iterable which contains the list of fields, sorted by index.
  Iterable<SchemaField> get fields => _fields.entries.orderBy((element) => element.key).map((e) => e.value);

  SchemaFieldSet<T> clone() {
    return SchemaFieldSet<T>._fromOther(_fields, _fieldsByName);
  }
}

class SchemaFieldSetBuilder<T> {
  final SplayTreeMap<int, SchemaField> _fields = SplayTreeMap();
  final SplayTreeMap<String, SchemaField> _fieldsByName = SplayTreeMap();

  SchemaFieldSetBuilder<T> addField<TField>(SchemaField<TField> field) {
    _fields[field.index] = field;
    _fieldsByName[field.dartName] = field;
    return this;
  }
}