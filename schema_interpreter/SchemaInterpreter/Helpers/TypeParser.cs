using System.Globalization;

namespace SchemaInterpreter.Helpers
{
    public static class TypeParser
    {
        public static int Int(string input, string throwMessage)
        {
            try
            {
                return int.Parse(input, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw Check.InvalidSchema(throwMessage);
            }
        }
    }
}