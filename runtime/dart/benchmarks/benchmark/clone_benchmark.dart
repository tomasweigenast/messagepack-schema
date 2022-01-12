import 'dart:typed_data';

import 'package:benchmark/benchmark.dart';
import 'package:messagepack_schema/messagepack_schema.dart';

void main() {
  late SchemaFieldSet fieldSet;
  setUp(() {
    fieldSet = SchemaFieldSet(SchemaFieldSetBuilder()
      .addField(SchemaField<String>("string_value", "stringValue", 0, SchemaFieldValueType.string, false, null, null))
      .addField(SchemaField<int>("int_value", "intValue", 1, SchemaFieldValueType.int64, false, null, null))
      .addField(SchemaField<double>("double_value", "doubleValue", 2, SchemaFieldValueType.float64, false, null, null))
      .addField(SchemaField.list<double>("list_double_value", "listDoubleValue", 3, SchemaFieldValueType.float64, null))
      .addField(SchemaField.map<int, String>("map_int_string_value", "mapIntStringValue", 4, SchemaFieldValueType.int64, SchemaFieldValueType.string, null))
      .addField(SchemaField<bool>("bool_value", "boolValue", 5,  SchemaFieldValueType.boolean, false, null, null))
      .addField(SchemaField<Uint8List>("binary_value", "binaryValue", 6, SchemaFieldValueType.binary, false, null, null))
    );
  });

  group("Copy SchemaFieldSet when creating new types", () {
    benchmark("Copy SchemaFieldSet without values.", () {
      fieldSet.clone();
    }, iterations: 100);
  });
}