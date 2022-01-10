import 'dart:convert';
import 'dart:typed_data';

import 'package:collection/collection.dart';
import 'package:messagepack_schema/messagepack_schema.dart';
import 'package:test/test.dart';

void main() {
  test("Test json encode", () {
    var expectedJson = {
      "stringValue": "an amazing string value",
      "intValue": 36554654156153,
      "doubleValue": 2121.215415183451,
      "listDoubleValue": [12, 25, 354, 21, 215, 51, 2],
      "mapIntStringValue": {
        5: "an amazing 5 value",
        2158451: "an amazing (maybe) long value"
      },
      "boolValue": true,
      "binaryValue": base64.encode([21, 51,51, 12,15,12 ,12, 12,12,1,21]),
    };

    ExampleType example = ExampleType(
      stringValue: "an amazing string value", 
      intValue: 36554654156153, 
      doubleValue: 2121.215415183451,
      mapIntStringValue: {
        5: "an amazing 5 value",
        2158451: "an amazing (maybe) long value"
      },
      listDoubleValue: [12, 25, 354, 21, 215, 51, 2],
      boolValue: true,
      binaryValue: Uint8List.fromList([21, 51,51,12,15,12 ,12, 12,12,1,21]),
    );

    var encodedJson = example.toJson();
    var deepEquals = const DeepCollectionEquality().equals;

    expect(deepEquals(encodedJson, expectedJson), true);
  });
}

class ExampleType extends SchemaType<ExampleType> {
  static final SchemaTypeInfo<ExampleType> _exampleTypeInfo = SchemaTypeInfo(
    fullName: "example.Example",
    fieldSet: SchemaFieldSet({
      0: SchemaField<String>(
        name: "string_value", 
        dartName: "stringValue",
        index: 0, 
        valueType: SchemaFieldValueType.string, 
        isNullable: false, 
        defaultValue: null, 
        customBuilder: null
      ),
      1: SchemaField<int>(
        name: "int_value",
        dartName: "intValue",
        index: 1,
        valueType: SchemaFieldValueType.int64,
        isNullable: false,
        customBuilder: null,
        defaultValue: null
      ),
      2: SchemaField<double>(
        name: "double_value",
        dartName: "doubleValue",
        index: 2,
        valueType: SchemaFieldValueType.float64,
        isNullable: false,
        defaultValue: null,
        customBuilder: null
      ),
      3: SchemaField<List<double>>(
        name: "list_double_value",
        index: 3,
        dartName: "listDoubleValue",
        valueType: SchemaFieldValueType.list(SchemaFieldValueType.float64),
        isNullable: true,
        defaultValue: null,
        customBuilder: null
      ),
      4: SchemaField<Map<int, String>>(
        name: "map_int_string_value",
        index: 4,
        dartName: "mapIntStringValue",
        valueType: SchemaFieldValueType.map(SchemaFieldValueType.int64, SchemaFieldValueType.string),
        isNullable: true,
        defaultValue: null,
        customBuilder: null
      ),
      5: SchemaField<bool>(
        name: "bool_value",
        dartName: "boolValue",
        index: 5,
        valueType: SchemaFieldValueType.boolean,
        isNullable: false,
        customBuilder: null,
        defaultValue: null
      ),
      6: SchemaField<Uint8List>(
        name: "binary_value",
        dartName: "binaryValue",
        index: 6,
        valueType: SchemaFieldValueType.binary,
        isNullable: false,
        customBuilder: null,
        defaultValue: null
      )
    })
  );
  
  @override
  SchemaTypeInfo<ExampleType> get info_ => _exampleTypeInfo;

  ExampleType._();

  String get stringValue => readValue_(0);
  set stringValue(String value) => setValue_(0, value);

  int get intValue => readValue_(1);
  set intValue(int value) => setValue_(1, value);

  double get doubleValue => readValue_(2);
  set doubleValue(double value) => setValue_(2, value);

  List<double>? get listDoubleValue => readValue_(3);
  set listDoubleValue(List<double>? value) => setValue_(3, value);

  Map<int, String>? get mapIntStringValue => readValue_(4);
  set mapIntStringValue(Map<int, String>? value) => setValue_(4, value);

  bool get boolValue => readValue_(5);
  set boolValue(bool value) => setValue_(5, value);

  Uint8List get binaryValue => readValue_(6);
  set binaryValue(Uint8List value) => setValue_(6, value);

  factory ExampleType({
    required String stringValue, 
    required int intValue, 
    required double doubleValue, 
    List<double>? listDoubleValue, 
    Map<int, String>? mapIntStringValue,
    required Uint8List binaryValue,
    required bool boolValue}) {
    var instance = createNew();
    instance.stringValue = stringValue;
    instance.intValue = intValue;
    instance.doubleValue = doubleValue;
    instance.listDoubleValue = listDoubleValue ?? instance.info_.fieldSet[3]!.defaultValue;
    instance.mapIntStringValue = mapIntStringValue ?? instance.info_.fieldSet[4]!.defaultValue;
    instance.binaryValue = binaryValue;
    instance.boolValue = boolValue;
    
    return instance;
  }

  static ExampleType createNew() {
    return ExampleType._();
  }
}