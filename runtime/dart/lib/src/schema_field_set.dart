part of '../messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final Map<int, SchemaField> _fields;

  const SchemaFieldSet(SplayTreeMap<int, SchemaField> fields) : _fields = fields;

  operator [](int i) => _fields[i];

  /// Returns an iterable which contains the list of fields, sorted by index.
  Iterable<SchemaField> get fields => _fields.entries.orderBy((element) => element.key).map((e) => e.value);
}