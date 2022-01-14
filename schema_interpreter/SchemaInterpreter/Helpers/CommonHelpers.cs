using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SchemaInterpreter.Helpers
{
    public static class CommonHelpers
    {
        public static bool? IsDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;
            else if (File.Exists(path))
                return false;
            else return null;
        }

        public static string CalculateMD5(string input)
        {
            if (input.IsNullOrWhitespace())
                return null;

            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            
            return stringBuilder.ToString();
        }
    }
}
