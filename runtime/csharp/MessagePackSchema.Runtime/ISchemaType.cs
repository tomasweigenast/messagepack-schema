namespace MessagePackSchema.Runtime
{
    /// <summary>
    /// Contains the basic type information of a schema type.
    /// </summary>
    public interface ISchemaType
    {
        /// <summary>
        /// Serializes the current type to an output stream.
        /// </summary>
        /// <param name="output">The stream to write to type to.</param>
        void Serialize(Stream output);

        /// <summary>
        /// Serializes the current type to an output stream asynchronously.
        /// </summary>
        /// <param name="output">The stream to write to type to.</param>
        Task SerializeAsync(Stream output);

        /// <summary>
        /// Deserializes an input stream and merges to the current type.
        /// </summary>
        /// <param name="input">The input to deserialize.</param>
        void MergeFrom(Stream input);

        /// <summary>
        /// Deserializes an input stream and merges to the current type asynchronously.
        /// </summary>
        /// <param name="input">The input to deserialize.</param>
        Task MergeFromAsync(Stream input);
    }

    /// <summary>
    /// Generic implementation of <see cref="ISchemaType"/>.
    /// </summary>
    public interface ISchemaType<T> : ISchemaType, IEquatable<T>, IClonableType<T> where T : ISchemaType<T>
    {
        /// <summary>
        /// Merges the current type instance with other instance.
        /// </summary>
        /// <param name="other">The instance to merge with this one.</param>
        void MergeUsing(T other);
    }
}