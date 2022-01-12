import 'package:messagepack_schema/messagepack_schema.dart';

class ExampleTypeEnum extends SchemaTypeEnum {

  static const ExampleTypeEnum unknown = ExampleTypeEnum._(0, 'unknown');
  static const ExampleTypeEnum first = ExampleTypeEnum._(1, 'first');
  static const ExampleTypeEnum second = ExampleTypeEnum._(2, 'second');
  static const ExampleTypeEnum randomValue = ExampleTypeEnum._(3, 'randomValue');

  const ExampleTypeEnum._(int index, String name) : super(index, name);

  static const List<ExampleTypeEnum> values = [
    unknown,
    first,
    second,
    randomValue
  ];

  static ExampleTypeEnum? maybeValueOf(int value){
    try {
      return values[value];
    } catch(_) {
      return null;
    }
  }
}