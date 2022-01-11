import 'dart:typed_data';

import 'package:messagepack_schema/messagepack_schema.dart';

final exampleFieldSet = SchemaFieldSet({
    0: SchemaField<String>("string_value", "stringValue", 0, SchemaFieldValueType.string, false, null, null),
    1: SchemaField<int>("int_value", "intValue", 1, SchemaFieldValueType.int64, false, null, null),
    2: SchemaField<double>("double_value", "doubleValue", 2, SchemaFieldValueType.float64, false, null, null),
    3: SchemaField.list<double>("list_double_value", "listDoubleValue", 3, SchemaFieldValueType.float64, null),
    4: SchemaField.map<int, String>("map_int_string_value", "mapIntStringValue", 4, SchemaFieldValueType.int64, SchemaFieldValueType.string, null),
    5: SchemaField<bool>("bool_value", "boolValue", 5,  SchemaFieldValueType.boolean, false, null, null),
    6: SchemaField<Uint8List>("binary_value", "binaryValue", 6, SchemaFieldValueType.binary, false, null, null)
  });