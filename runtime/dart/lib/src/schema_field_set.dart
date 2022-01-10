part of '../messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final Map<int, SchemaField> _fields;

  SchemaFieldSet(Map<int, SchemaField> fields) : _fields = SplayTreeMap.from(fields);

  SchemaField? operator [](int i) => _fields[i];

  /// Returns an iterable which contains the list of fields, sorted by index.
  Iterable<SchemaField> get fields => _fields.entries.orderBy((element) => element.key).map((e) => e.value);
}