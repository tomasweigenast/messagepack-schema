import 'dart:typed_data';

import 'package:test/expect.dart';
import 'package:test/scaffolding.dart';

import 'types/example_enum.dart';
import 'types/example_type.dart';
import 'types/example_union.dart';

void main() {
  group("Test equality comparison", () {
    test("Test hashcode between objects of the same instance", () {
      var a = AnotherType(stringValue: "an string value");
      expect(a == a, isTrue);

      var b = AnotherType(stringValue: "an string value");
      expect(b.info_ == b.info_, isTrue);
    });

    test("Test hashcode between different objects with same values", () {
      var a = AnotherType(stringValue: "an string value");
      var b = AnotherType(stringValue: "an string value");

      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      b.boolValue = false;
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      a.boolValue = false;
      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      b.boolValue = true;
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);
    });

    test("Equality between unions works", () {
      var a = AnotherType(stringValue: "an string value", unionValue: ExampleUnion(aNumber: 12));
      var b = AnotherType(stringValue: "an string value", unionValue: ExampleUnion(aNumber: 12));

      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      b.unionValue.aEnum = ExampleTypeEnum.randomValue;
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      b.unionValue.aNumber = 12;
      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);
    });

    test("List and map equality works", () {
      var a = ListAndMapType(
        listDoubleValue: [25, 12.25, 0.365],
        mapStringBoolValue: {
          "random": true,
          "year": false
        }
      );

      var b = ListAndMapType(
        listDoubleValue: [25, 12.25, 0.365],
        mapStringBoolValue: {
          "random": true,
          "year": false
        }
      );

      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      b.mapStringBoolValue.clear();
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      a.mapStringBoolValue.clear();
      b.mapStringBoolValue["hello-world"] = false;
      a.mapStringBoolValue["hello-world"] = false;
      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      a.listDoubleValue.addAll([5, 356.2451]);
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      b.listDoubleValue.addAll([5, 356.2451]);
      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);


    }, tags: ["map-list"]);

    test("Equality between types with nested types works", () {
      var a = ExampleType(
        boolValue: true,
        binaryValue: Uint8List.fromList([12, 25, 32]),
        doubleValue: 12.285,
        enumValue: ExampleTypeEnum.first,
        intValue: 1212,
        stringValue: "random string",
        anotherTypeValue: AnotherType(
          stringValue: "hello world",
          listDoubleValue: [12, 253.23],
          mapStringBoolValue: {
            "random": true
          },
          unionValue: ExampleUnion(
            aEnum: ExampleTypeEnum.randomValue,
          )
        )
      );  
      var b = ExampleType(
        boolValue: true,
        binaryValue: Uint8List.fromList([12, 25, 32]),
        doubleValue: 12.285,
        enumValue: ExampleTypeEnum.first,
        intValue: 1212,
        stringValue: "random string",
        anotherTypeValue: AnotherType(
          stringValue: "hello world",
          mapStringBoolValue: {
            "random": true
          },
          listDoubleValue: [12, 253.23],
          unionValue: ExampleUnion(
            aEnum: ExampleTypeEnum.randomValue,
          )
        )
      );  

      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);

      b.anotherTypeValue?.listDoubleValue.add(25);
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      b.anotherTypeValue?.listDoubleValue.remove(25);
      b.anotherTypeValue?.mapStringBoolValue["random key"] = true;
      expect(a == b, isFalse);
      expect(a.hashCode == b.hashCode, isFalse);

      a.anotherTypeValue?.mapStringBoolValue["random key"] = true;
      expect(a == b, isTrue);
      expect(a.hashCode == b.hashCode, isTrue);
    }, tags: ["nested"]);
  });
}