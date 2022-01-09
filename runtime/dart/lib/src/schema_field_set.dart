import 'dart:collection';

import 'package:messagepack_schema/messagepack_schema.dart';

/// Contains all the fields in a type.
class SchemaFieldSet<T> {
  final Map<int, SchemaField> _fields;

  const SchemaFieldSet(SplayTreeMap<int, SchemaField> fields) : _fields = fields;

  operator [](int i) => _fields[i];

  Iterable<SchemaField> get fields => _fields.values;
}