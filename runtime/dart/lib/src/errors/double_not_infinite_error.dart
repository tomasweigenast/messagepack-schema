class DoubleNotInfiniteError extends Error {
  @override
  String toString() => "While serializing to JSON, double values cannot be infinite.";
}