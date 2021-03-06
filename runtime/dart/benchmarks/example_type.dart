import 'dart:typed_data';

import 'package:messagepack_schema/messagepack_schema.dart';

class ExampleType extends SchemaType<ExampleType> {
  static final SchemaTypeInfo<ExampleType> _exampleTypeInfo = SchemaTypeInfo(
    fullName: "example.Example",
    fieldSet: SchemaFieldSet(SchemaFieldSetBuilder()
      .addField(SchemaField<String>("string_value", "stringValue", 0, SchemaFieldValueType.string, false, null, null))
      .addField(SchemaField<int>("int_value", "intValue", 1, SchemaFieldValueType.int64, false, null, null))
      .addField(SchemaField<double>("double_value", "doubleValue", 2, SchemaFieldValueType.float64, false, null, null))
      .addField(SchemaField.list<double>("list_double_value", "listDoubleValue", 3, SchemaFieldValueType.float64, null))
      .addField(SchemaField.map<int, String>("map_int_string_value", "mapIntStringValue", 4, SchemaFieldValueType.int64, SchemaFieldValueType.string, null))
      .addField(SchemaField<bool>("bool_value", "boolValue", 5,  SchemaFieldValueType.boolean, false, null, null))
      .addField(SchemaField<Uint8List>("binary_value", "binaryValue", 6, SchemaFieldValueType.binary, false, null, null))
    )
  );
  @override
  SchemaTypeInfo<ExampleType> get info_ => _exampleTypeInfo;

  ExampleType._() : super();

  String get stringValue => $readValue_(0);
  set stringValue(String value) => $setValue_(0, value);

  int get intValue => $readValue_(1);
  set intValue(int value) => $setValue_(1, value);

  double get doubleValue => $readValue_(2);
  set doubleValue(double value) => $setValue_(2, value);

  List<double>? get listDoubleValue => $readValue_(3);
  set listDoubleValue(List<double>? value) => $setValue_(3, value);

  Map<int, String>? get mapIntStringValue => $readValue_(4);
  set mapIntStringValue(Map<int, String>? value) => $setValue_(4, value);

  bool get boolValue => $readValue_(5);
  set boolValue(bool value) => $setValue_(5, value);

  Uint8List get binaryValue => $readValue_(6);
  set binaryValue(Uint8List value) => $setValue_(6, value);

  factory ExampleType({
    required String stringValue, 
    required int intValue, 
    required double doubleValue, 
    List<double>? listDoubleValue, 
    Map<int, String>? mapIntStringValue,
    required Uint8List binaryValue,
    required bool boolValue}) {
    var instance = createEmpty();
    instance.stringValue = stringValue;
    instance.intValue = intValue;
    instance.doubleValue = doubleValue;
    instance.listDoubleValue = listDoubleValue ?? instance.info_.fieldSet[3]!.defaultValue;
    instance.mapIntStringValue = mapIntStringValue ?? instance.info_.fieldSet[4]!.defaultValue;
    instance.binaryValue = binaryValue;
    instance.boolValue = boolValue;
    
    return instance;
  }

  static ExampleType createEmpty() {
    return ExampleType._();
  }
}