class InvalidValueError extends Error {
  final String message;

  InvalidValueError(this.message);

  @override
  String toString() => "Invalid value type: $message";
}