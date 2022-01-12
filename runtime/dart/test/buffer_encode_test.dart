import 'dart:typed_data';

import 'package:messagepack_schema/src/message_pack/packer.dart';
import 'package:test/test.dart';

import './types/example_enum.dart';
import './types/example_type.dart';

void main() {
  group("Test encoding and decoding to/from a buffer", () {
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
        .packBool(true)
        .packBinary(Uint8List.fromList([21, 51,51,12,15,12 ,12, 12,12,1,21]))
        .packInt(3)
        .packNull()
        .takeBytes();

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
        enumValue: ExampleTypeEnum.randomValue,
      );

      var buffer = example.toBuffer();

      expect(buffer, equals(expectedBuffer));
    }, tags: ["encode"]);

    test("Test buffer decode", () {
      var encodedBuffer = Packer()
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
        .packBool(true)
        .packBinary(Uint8List.fromList([21, 51,51,12,15,12 ,12, 12,12,1,21]))
        .packInt(2)
        .packBinary(AnotherType(
          stringValue: "amazing string from another type",
          boolValue: false,
          listDoubleValue: [25, 32.25, 65.23, 122.25]
        ).toBuffer())
        .takeBytes();

      var decodedExample = ExampleType.createEmpty()..mergeFromBuffer(encodedBuffer);

      expect(decodedExample.stringValue, equals("an amazing string value"));
      expect(decodedExample.intValue, equals(36554654156153));
      expect(decodedExample.doubleValue, equals(2121.215415183451));
      expect(decodedExample.mapIntStringValue, equals({5: "an amazing 5 value",2158451: "an amazing (maybe) long value"}));
      expect(decodedExample.listDoubleValue, equals([12, 25, 354, 21, 215, 51, 2]));
      expect(decodedExample.boolValue, equals(true));
      expect(decodedExample.binaryValue, equals([21, 51,51,12,15,12 ,12, 12,12,1,21]));
      expect(decodedExample.enumValue, equals(ExampleTypeEnum.second));
      expect(decodedExample.anotherTypeValue, isNotNull);
      expect(decodedExample.anotherTypeValue!.stringValue, equals("amazing string from another type"));
      expect(decodedExample.anotherTypeValue!.boolValue, equals(false));
      expect(decodedExample.anotherTypeValue!.listDoubleValue, equals([25, 32.25, 65.23, 122.25]));
    }, tags: ["decode"]);
  });
}