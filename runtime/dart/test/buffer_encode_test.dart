import 'dart:typed_data';

import 'package:collection/collection.dart';
import 'package:test/test.dart';
import 'package:messagepack_schema/messagepack_schema.dart';
import 'package:messagepack_schema/src/message_pack/packer.dart';

void main() {
  test("Test buffer encode", () {
    var expectedBuffer = Packer()
      .packString("an amazing string value")
      .packInt(36554654156153)
      .packDouble(2121.215415183451)
      .packListLength(7)
      .packDouble(12)
      .packDouble(25)
      .packDouble(354)
      .packDouble(21)
      .packDouble(215)
      .packDouble(51)
      .packDouble(2)
      .packMapLength(2)
      .packInt(5)
      .packString("an amazing 5 value")
      .packInt(2158451)
      .packString("an amazing (maybe) long value")
      .takeBytes();

      // TODO: Finish test

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

    var buffer = example.toBuffer();
    var deepEquals = const DeepCollectionEquality().equals;

    expect(deepEquals(buffer, expectedBuffer), true);
  });
}

class ExampleType extends SchemaType<ExampleType> {
  static final SchemaTypeInfo<ExampleType> _exampleTypeInfo = SchemaTypeInfo(
    fullName: "example.Example",
    fieldSet: SchemaFieldSet({
      0: SchemaField<String>("string_value", "stringValue", 0, SchemaFieldValueType.string, false, null, null),
      1: SchemaField<int>("int_value", "intValue", 1, SchemaFieldValueType.int64, false, null, null),
      2: SchemaField<double>("double_value", "doubleValue", 2, SchemaFieldValueType.float64, false, null, null),
      3: SchemaField.list<double>("list_double_value", "listDoubleValue", 3, SchemaFieldValueType.float64, null),
      4: SchemaField.map<int, String>("map_int_string_value", "mapIntStringValue", 4, SchemaFieldValueType.int64, SchemaFieldValueType.string, null),
      5: SchemaField<bool>("bool_value", "boolValue", 5,  SchemaFieldValueType.boolean, false, null, null),
      6: SchemaField<Uint8List>("binary_value", "binaryValue", 6, SchemaFieldValueType.binary, false, null, null)
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