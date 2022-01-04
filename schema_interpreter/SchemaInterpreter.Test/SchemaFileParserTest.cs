using SchemaInterpreter.Parser;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SchemaInterpreter.Test
{
    public class SchemaFileParserTest
    {
        [Fact]
        public async Task TestFileParser()
        {
            var schema = new StreamReader(File.OpenRead("test1.mpack"));
            var parsedSchema = await SchemaFileParser.ParseFile(schema);
        }
    }
}