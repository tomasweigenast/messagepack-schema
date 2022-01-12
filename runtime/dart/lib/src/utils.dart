// Hashing library taken from https://github.com/google/quiver-dart/blob/master/lib/src/core/hash.dart
part of '../messagepack_schema.dart';

class _Hashing {
  const _Hashing();

  static int hashObjects(Iterable objects) =>
    finish(objects.fold(0, (h, i) => combine(h, i.hashCode)));

  // Jenkins hash functions

  static int combine(int hash, int value) {
    hash = 0x1fffffff & (hash + value);
    hash = 0x1fffffff & (hash + ((0x0007ffff & hash) << 10));
    return hash ^ (hash >> 6);
  }

  static int finish(int hash) {
    hash = 0x1fffffff & (hash + ((0x03ffffff & hash) << 3));
    hash = hash ^ (hash >> 11);
    return 0x1fffffff & (hash + ((0x00003fff & hash) << 15));
  }
}