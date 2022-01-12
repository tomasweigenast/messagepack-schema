import 'package:test/expect.dart';
import 'package:test/scaffolding.dart';

import './types/example_enum.dart';

void main() {
  group("Test enums", () {
    test("Enums should be equals", () {
      var first = ExampleTypeEnum.first;
      var anotherFirst = ExampleTypeEnum.first;

      expect(first, equals(anotherFirst));
      expect(ExampleTypeEnum.randomValue, isNot(equals(ExampleTypeEnum.unknown)));
    });

    test("Enums should print their name when calling toString()", () {
      expect(ExampleTypeEnum.randomValue.toString(), equals("randomValue"));
    });
  });
}