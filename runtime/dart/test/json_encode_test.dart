import 'dart:convert';
import 'dart:typed_data';

import 'package:test/test.dart';

import './types/example_enum.dart';
import './types/example_type.dart';
import 'types/example_union.dart';

void main() {
  group("Test encoding and decoding to/from json", () {
    test("Test json encode", () {
      var encodedJson = ExampleType(
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
        enumValue: ExampleTypeEnum.randomValue,
        anotherTypeValue: AnotherType(
          stringValue: "an amazing another type string value",
          boolValue: false,
          mapStringBoolValue: {
            "first": true,
            "me?": false,
            "you?": true
          },
          unionValue: ExampleUnion(
            aString: "a string in a union, yeah!"
          )
        )
      ).toJson();

      var expectedJson = {
        "stringValue": "an amazing string value",
        "intValue": 36554654156153,
        "doubleValue": 2121.215415183451,
        "listDoubleValue": <double>[12, 25, 354, 21, 215, 51, 2],
        "mapIntStringValue": {
          "5": "an amazing 5 value",
          "2158451": "an amazing (maybe) long value"
        },
        "boolValue": true,
        "binaryValue": base64.encode([21, 51,51, 12,15,12 ,12, 12,12,1,21]),
        "enumValue": 3,
        "anotherTypeValue": {
          "stringValue": "an amazing another type string value",
          "boolValue": false,
          "listDoubleValue": [],
          "mapStringBoolValue": {
            "first": true,
            "me?": false,
            "you?": true
          },
          "aString": "a string in a union, yeah!"
        }
      };

      expect(encodedJson, equals(expectedJson));
    }, tags: "encode");

    test("Test json decode", () {
      var inputJson = json.decode(json.encode({
        "stringValue": "an amazing string value",
        "intValue": 36554654156153,
        "doubleValue": 2121.215415183451,
        "listDoubleValue": <double>[12, 25, 354, 21, 215, 51, 2],
        "mapIntStringValue": {
          "5": "an amazing 5 value",
          "2158451": "an amazing (maybe) long value"
        },
        "boolValue": true,
        "binaryValue": base64.encode([21, 51,51, 12,15,12 ,12, 12,12,1,21]),
        "enumValue": 2,
        "anotherTypeValue": {
          "stringValue": "an amazing another type string value",
          "boolValue": false,
          "listDoubleValue": [],
          "mapStringBoolValue": {
            "first": true,
            "me?": false,
            "you?": true
          },
          "aEnum": 2
        }
      }));

      var decodedType = ExampleType.createEmpty()..mergeFromJson(inputJson);
      expect(decodedType.stringValue, equals("an amazing string value"));
      expect(decodedType.intValue, equals(36554654156153));
      expect(decodedType.doubleValue, equals(2121.215415183451));
      expect(decodedType.listDoubleValue, equals([12, 25, 354, 21, 215, 51, 2]));
      expect(decodedType.mapIntStringValue, equals({5: "an amazing 5 value", 2158451: "an amazing (maybe) long value"}));
      expect(decodedType.boolValue, isTrue);
      expect(decodedType.binaryValue, equals(Uint8List.fromList([21, 51,51, 12,15,12 ,12, 12,12,1,21])));
      expect(decodedType.enumValue, equals(ExampleTypeEnum.second));
      expect(decodedType.anotherTypeValue, isNotNull);
      expect(decodedType.anotherTypeValue!.stringValue, equals("an amazing another type string value"));
      expect(decodedType.anotherTypeValue!.boolValue, equals(false));
      expect(decodedType.anotherTypeValue!.listDoubleValue, isEmpty);
      expect(decodedType.anotherTypeValue!.mapStringBoolValue, equals({"first": true, "me?": false, "you?": true}));
      expect(decodedType.anotherTypeValue!.enumValue, isNull);
      expect(decodedType.anotherTypeValue!.unionValue.aEnum, equals(ExampleTypeEnum.second));
    }, tags: "decode");
  });
}