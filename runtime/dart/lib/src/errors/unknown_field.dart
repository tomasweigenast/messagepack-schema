class UnknownTypeField extends Error {
  final int fieldIndex;

  UnknownTypeField(this.fieldIndex);

  @override
  String toString() => "Unknown field with index $fieldIndex";
}