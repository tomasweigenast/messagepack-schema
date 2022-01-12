import 'dart:typed_data';

import 'package:messagepack_schema/messagepack_schema.dart';

import 'example_enum.dart';
import 'example_union.dart';

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
      .addField(SchemaField.enumerator<ExampleTypeEnum>("enum_value", "enumValue", 7, false, ExampleTypeEnum.unknown, ExampleTypeEnum.maybeValueOf))
      .addField(SchemaField.type<AnotherType>("another_type_value", "anotherTypeValue", "AnotherType", 8, true, AnotherType.createEmpty))
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

  List<double> get listDoubleValue => $readValue_(3);
  set listDoubleValue(List<double> value) => $setValue_(3, value);

  Map<int, String> get mapIntStringValue => $readValue_(4);
  set mapIntStringValue(Map<int, String> value) => $setValue_(4, value);

  bool get boolValue => $readValue_(5);
  set boolValue(bool value) => $setValue_(5, value);

  Uint8List get binaryValue => $readValue_(6);
  set binaryValue(Uint8List value) => $setValue_(6, value);

  ExampleTypeEnum get enumValue => $readValue_(7);
  set enumValue(ExampleTypeEnum value) => $setValue_(7, value);

  AnotherType? get anotherTypeValue => $readValue_(8);
  set anotherTypeValue(AnotherType? value) => $setValue_(8, value);

  factory ExampleType({
    required String stringValue, 
    required int intValue, 
    required double doubleValue, 
    List<double>? listDoubleValue, 
    Map<int, String>? mapIntStringValue,
    required Uint8List binaryValue,
    required bool boolValue,
    required ExampleTypeEnum enumValue,
    AnotherType? anotherTypeValue}) {
    var instance = ExampleType.createEmpty();
    instance.stringValue = stringValue;
    instance.intValue = intValue;
    instance.doubleValue = doubleValue;
    instance.listDoubleValue = listDoubleValue ?? instance.info_.fieldSet[3]!.defaultValue;
    instance.mapIntStringValue = mapIntStringValue ?? instance.info_.fieldSet[4]!.defaultValue;
    instance.binaryValue = binaryValue;
    instance.boolValue = boolValue;
    instance.enumValue = enumValue;
    instance.anotherTypeValue = anotherTypeValue;
    
    return instance;
  }

  factory ExampleType.createEmpty() {
    return ExampleType._();
  }
}

class AnotherType extends SchemaType<AnotherType> {
  static final SchemaTypeInfo<AnotherType> _anotherTypeInfo = SchemaTypeInfo(
    fullName: "example.AnotherType",
    fieldSet: SchemaFieldSet(SchemaFieldSetBuilder()
      .addField(SchemaField<String>("string_value", "stringValue", 0, SchemaFieldValueType.string, false, null, null))
      .addField(SchemaField.list<double>("list_double_value", "listDoubleValue", 1, SchemaFieldValueType.float64, null))
      .addField(SchemaField.map<String, bool>("map_string_bool_value", "mapStringBoolValue", 2, SchemaFieldValueType.string, SchemaFieldValueType.boolean, null))
      .addField(SchemaField<bool>("bool_value", "boolValue", 3,  SchemaFieldValueType.boolean, true, null, null))
      .addField(SchemaField.enumerator<ExampleTypeEnum>("enum_value", "enumValue", 4, true, ExampleTypeEnum.unknown, ExampleTypeEnum.maybeValueOf))
      .addField(SchemaField.union<ExampleUnion>("union_value", "unionValue", "ExampleUnion", 5, ExampleUnion.$fieldNames_, ExampleUnion.new, (u) => u.fieldName()))
    )
  );
  
  @override
  SchemaTypeInfo<AnotherType> get info_ => _anotherTypeInfo;

  AnotherType._() : super();

  String get stringValue => $readValue_(0);
  set stringValue(String value) => $setValue_(0, value);

  List<double> get listDoubleValue => $readValue_(1);
  set listDoubleValue(List<double> value) => $setValue_(1, value);

  Map<String, bool> get mapStringBoolValue => $readValue_(2);
  set mapStringBoolValue(Map<String, bool> value) => $setValue_(2, value);

  bool? get boolValue => $readValue_(3);
  set boolValue(bool? value) => $setValue_(3, value);

  ExampleTypeEnum? get enumValue => $readValue_(4);
  set enumValue(ExampleTypeEnum? value) => $setValue_(4, value);

  ExampleUnion get unionValue => $readValue_(5);
  set unionValue(ExampleUnion value) => $setValue_(5, value);

  factory AnotherType({
    required String stringValue, 
    List<double>? listDoubleValue, 
    Map<String, bool>? mapStringBoolValue,
    bool? boolValue,
    ExampleTypeEnum? enumValue,
    ExampleUnion? unionValue}) {
    var instance = AnotherType.createEmpty();
    instance.stringValue = stringValue;
    instance.listDoubleValue = listDoubleValue ?? instance.info_.fieldSet[1]!.defaultValue;
    instance.mapStringBoolValue = mapStringBoolValue ?? instance.info_.fieldSet[2]!.defaultValue;
    instance.boolValue = boolValue;
    instance.enumValue = enumValue;
    
    if(unionValue != null) {
      instance.unionValue = unionValue;
    }

    return instance;
  }

  factory AnotherType.createEmpty() {
    return AnotherType._();
  }
}