import 'package:messagepack_schema/src/errors/union_error.dart';
import 'package:test/test.dart';

import './types/example_enum.dart';
import './types/example_union.dart';

void main() {
  group("Test unions", () {

    test("When union constructors specify more than one value, the value to set is selected in order of field indexes.", () {
      var union = ExampleUnion(
        aEnum: ExampleTypeEnum.first,
        aString: "A random string"
      );

      expect(union.aString, equals("A random string"));
      expect(() => union.aEnum, throwsA(isA<UnionError>()));
      expect(() => union.aNumber, throwsA(isA<UnionError>()));
      expect(() => union.aExampleType, throwsA(isA<UnionError>()));
    });

    test("Unions should admit one value at time.", () {
      var union = ExampleUnion(
        aNumber: 254131
      );

      expect(union.aNumber, equals(254131));
      
      union.aEnum = ExampleTypeEnum.randomValue;
      expect(() => union.aNumber, throwsA(isA<UnionError>()));
      expect(union.aEnum, equals(ExampleTypeEnum.randomValue));
    });

    test("currentUnionType should return the current field set.", () {
      var union = ExampleUnion(
        aNumber: 254131
      );

      expect(union.currentUnionType, equals(ExampleUnionTypes.aNumber));
    });

    test("clear() clears the current union value.", () {
      var union = ExampleUnion(
        aEnum: ExampleTypeEnum.second
      );

      expect(union.aEnum, equals(ExampleTypeEnum.second));

      union.clearUnion();

      expect(union.isUnset, isTrue);
      expect(union.currentUnionType, equals(ExampleUnionTypes.unset));
    });
  }); 
}