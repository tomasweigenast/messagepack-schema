import 'dart:typed_data';

/// Provides methods to encode/decode to/from JSON and Uint8List
abstract class Encodable {
  /// Writes the current type instance to a buffer.
  Uint8List toBuffer();

  /// Merges a encoded messagepack buffer to the current type instance.
  void mergeFromBuffer(Uint8List buffer);

  /// Serializes the current type instance to json.
  Object? toJson();

  /// Merges a encoded JSON map to the current type instance.
  void mergeFromJson(Object? map);
}

/// An abstraction over [Encodable] which provides a method to set custom name to fields while encoding to JSON.
abstract class NamedEncodable extends Encodable {
  
  /// Returns the name of the field.
  String fieldName();
}