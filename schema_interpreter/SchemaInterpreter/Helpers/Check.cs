using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Parser;

namespace SchemaInterpreter.Helpers
{
    public static class Check
    {
        /// <summary>
        /// Throws a new <see cref="InvalidSchemaException"/>
        /// </summary>
        public static void ThrowInvalidSchema(string message, bool includeLineInfo = true)
        {
            throw new InvalidSchemaException(message, includeLineInfo ? ParserContext.Current?.CurrentLine : null);
        }

        /// <summary>
        /// Creates and returns a new <see cref="InvalidSchemaException"/>
        /// </summary>
        public static InvalidSchemaException InvalidSchema(string message, bool includeLineInfo = true)
        {
            return new InvalidSchemaException(message, includeLineInfo ? ParserContext.Current?.CurrentLine : null);
        }

        public static void ThrowInternal(string message)
        {
            throw new InternalException(message);
        }

        public static InternalException Internal(string message)
        {
            return new InternalException(message);
        }
    }
}