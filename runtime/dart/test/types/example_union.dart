import 'package:messagepack_schema/messagepack_schema.dart';

import 'example_enum.dart';
import 'example_type.dart';

class ExampleUnion extends SchemaTypeUnion<ExampleUnion> {

  static const Map<int, ExampleUnionTypes> _unionTypes = {
    0: ExampleUnionTypes.aNumber,
    1: ExampleUnionTypes.aString,
    2: ExampleUnionTypes.aExampleType,
    3: ExampleUnionTypes.aEnum,
    -1: ExampleUnionTypes.unset,
  };

  static const List<String> _unionFieldNames = [
    "aNumber",
    "aString",
    "aExampleType",
    "aEnum"
  ];

  static List<String> get $fieldNames_ => _unionFieldNames;

  static final SchemaFieldSet<ExampleUnion> _fieldSet = SchemaFieldSet(SchemaFieldSetBuilder()
    .addField(SchemaField<int>("a_number", "aNumber", 0, SchemaFieldValueType.int64, false, null, null))
    .addField(SchemaField<String>("a_string", "aString", 1, SchemaFieldValueType.string, false, null, null))
    .addField(SchemaField.type<ExampleType>("a_example_type", "aExampleType", "ExampleType", 2, false, ExampleType.createEmpty))
    .addField(SchemaField.enumerator<ExampleTypeEnum>("a_enum", "aEnum", 3, false, ExampleTypeEnum.unknown, ExampleTypeEnum.maybeValueOf))
  );

  @override
  SchemaFieldSet<ExampleUnion> get fieldSet_ => _fieldSet;

  int get aNumber => $value_(0);
  set aNumber(int value) => $setValue_(0, value);

  String get aString => $value_(1);
  set aString(String value) => $setValue_(1, value);

  ExampleType get aExampleType => $value_(2);
  set aExampleType(ExampleType value) => $setValue_(2, value);

  ExampleTypeEnum get aEnum => $value_(3);
  set aEnum(ExampleTypeEnum value) => $setValue_(3, value);

  /// Gets the current union type set.
  ExampleUnionTypes get currentUnionType => _unionTypes[$whichField_()]!;

  ExampleUnion._() : super();

  factory ExampleUnion({
    int? aNumber,
    String? aString,
    ExampleType? aExampleType,
    ExampleTypeEnum? aEnum
  }) {
    var instance = ExampleUnion._();
    if(aNumber != null) {
      instance.aNumber = aNumber;
    } else if(aString != null) {
      instance.aString = aString;
    } else if(aExampleType != null) {
      instance.aExampleType = aExampleType;
    } else if(aEnum != null) {
      instance.aEnum = aEnum;
    }

    return instance;
  }
}

enum ExampleUnionTypes {
  aNumber, aString, aExampleType, aEnum, unset
}