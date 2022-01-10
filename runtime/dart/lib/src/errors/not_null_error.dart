class NotNullError extends Error {

  final String fieldName;
  final String typeName;

  NotNullError({required this.fieldName, required this.typeName});

  @override
  String toString() => "Field $fieldName of type $typeName expected to have a value.";
}