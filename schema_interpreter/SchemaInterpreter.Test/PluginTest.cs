using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Plugin.Encoder;
using System.Text;
using Xunit;

namespace SchemaInterpreter.Test
{
    public class PluginTest
    {
        [Fact]
        public void Test_encode_json()
        {
            var context = ParserContext.CreateContext();
            context.CurrentPackage = new SchemaPackage("package1", 1);
            context.CurrentPackage.AddType(new SchemaType("Package1Model", "package1", null, new[]
            {
                new SchemaTypeField("random_field", 0, SchemaTypeFieldValueType.Parse("string"), null, false, null, null),
                new SchemaTypeField("another", 1, SchemaTypeFieldValueType.Parse("int32"), 32, false, null, null),
                new SchemaTypeField("nullable_field", 1, SchemaTypeFieldValueType.Parse("boolean"), null, true, null, null),
            }));

            context.CurrentPackage = new SchemaPackage("package2", 1);
            context.CurrentPackage.Imports.Add("package1");
            context.CurrentPackage.AddType(new SchemaType("Package2Model", "package2", null, new[]
            {
                new SchemaTypeField("another_field", 0, SchemaTypeFieldValueType.Custom("package1.Package1Model"), null, true, null, null),
                new SchemaTypeField("list_field", 1, SchemaTypeFieldValueType.Parse("list(string)"), null, false, null, null),
                new SchemaTypeField("map_field", 2, SchemaTypeFieldValueType.Parse("map(string,float64)"), -123456.241561654, false, null, null),
            }));

            // Do a import rename
            context.RenameImports();

            var jsonSchemaEncoder = new JsonSchemaEncoder();
            var buffer = jsonSchemaEncoder.Encode(PluginEncoding.Json, context.Packages).Result;
            var jsonString = Encoding.UTF8.GetString(buffer.ToArray());
            var output = (JObject)JsonConvert.DeserializeObject(jsonString);

            var packagesArray = (JArray)output["packages"];
            Assert.Equal(2, packagesArray.Count);

            var firstPackage = packagesArray[0];
            Assert.Equal(CommonHelpers.CalculateMD5("package1"), firstPackage["id"]);
            Assert.Equal("package1", firstPackage["name"]);
            Assert.Equal(1, firstPackage["version"]);
            Assert.Empty(firstPackage["imports"]);
        }
    }
}