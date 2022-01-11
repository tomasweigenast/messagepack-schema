import 'dart:typed_data';

import 'package:benchmark/benchmark.dart';

import 'example_field_set.dart';
import 'example_type.dart';

void main() {
  benchmark("Copy SchemaFieldSet with every SchemaFieldValue without its actual value when creating new types.", () {
    exampleFieldSet.clone();
  }, iterations: 1000000);

  group("Buffer encode/decode", () {
    late ExampleType type;
    late Uint8List typeAsBuffer;
    setUp(() {
      type = ExampleType(
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

      typeAsBuffer = type.toBuffer();
    });

    benchmark("Encode to buffer", () {
      type.toBuffer();
    }, iterations: 1000000);

    benchmark("Decode from buffer", () {
      ExampleType.createEmpty().mergeFromBuffer(typeAsBuffer);
    }, iterations: 1000000);
  });

  benchmark("Copy SchemaFieldSet with every SchemaFieldValue without its actual value when creating new types.", () {
    exampleFieldSet.clone();
  }, iterations: 1000000);
}