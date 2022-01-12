class UnionError extends Error {
  final String message;

  UnionError(this.message);

  @override
  String toString() => "Union error: $message";
}