using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Parser;
using SchemaInterpreter.Parser.Builder;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Parser.V1;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SchemaInterpreter.Test
{
    public class SchemaFileParserV1Test
    {
        [Fact]
        public void Test_parse_fields()
        {
            var context = ParserContext.CreateContext();
            context.CurrentTypeBuilder = new SchemaTypeBuilder();

            string line = "my_field:string 0";
            SchemaTypeField field = SchemaFileParserV1.ReadField(line);

            Assert.Equal("my_field", field.Name);
            Assert.Equal("string", field.ValueType.TypeName);
            Assert.Equal(0, field.Index);
            Assert.False(field.IsNullable);
            Assert.Null(field.Metadata);
            Assert.Null(field.DefaultValue);

            line = "my_field?:int32 1";
            field = SchemaFileParserV1.ReadField(line);

            Assert.Equal("my_field", field.Name);
            Assert.Equal("int32", field.ValueType.TypeName);
            Assert.Equal(1, field.Index);
            Assert.True(field.IsNullable);

            line = "my_field?:list(  string ) 1";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(SchemaTypeFieldValueType.List(SchemaTypeFieldValueType.Primitive("string")), field.ValueType);
            Assert.True(field.IsNullable);

            line = "my_field:map(int8 ,   string)   1";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Map(SchemaTypeFieldValueType.Primitive("int8"), SchemaTypeFieldValueType.Primitive("string")), field.ValueType);
            Assert.False(field.IsNullable);

            line = "my_field:list(string)1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:map(int8,list(string)) 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:map(map(string,int),list(string)) 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:list(map(string,int)) 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:list(MyEnum) 1";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal(SchemaTypeFieldValueType.List(SchemaTypeFieldValueType.Custom("MyEnum")), field.ValueType);

            line = "my_field:int 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "   my_field  : int32 1";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Primitive("int32"), field.ValueType);
            Assert.False(field.IsNullable);

            context.CurrentTypeBuilder = new SchemaTypeBuilder().SetModifier(SchemaTypeModifier.Enum);

            line = "random_value 1";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("random_value", field.Name);
            Assert.Equal(1, field.Index);
            Assert.False(field.IsNullable);

            line = "random_value? 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "random_value:string 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            context.CurrentTypeBuilder = new SchemaTypeBuilder().SetModifier(SchemaTypeModifier.Union);
            line = "my_field?:list(string) 1";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            context.CurrentTypeBuilder = new SchemaTypeBuilder();

            line = "my_field:string 1 = \"my amazing string\"";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Primitive("string"), field.ValueType);
            Assert.False(field.IsNullable);
            Assert.Equal("\"my amazing string\"", field.DefaultValue);

            line = "my_field?:int32 1=24";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Primitive("int32"), field.ValueType);
            Assert.True(field.IsNullable);
            Assert.Equal("24", field.DefaultValue);

            line = "my_field:int32 1 @([\"obsolete\": true],[\"another\":23],[\"hey\":\"another key\"],[\"doubleKey\"]:25.25)";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Primitive("int32"), field.ValueType);
            Assert.Equal(new Dictionary<string, object> {
                { "obsolete", true },
                { "another", (long)23 },
                { "hey", "another key" },
                { "doubleKey", 25.25 }
            }, field.Metadata);

            line = "my_field:int32 1 @([25: true])";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:int32 1 @([\"invalidPair\"])";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:int32 1 @([\"tooManyArguments\":25:32])";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:int32 1 @[\"tooManyArguments\":25:32]";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:int32 1 @([\"invalidMetadataBeforeDefaultValue\": true]) = 2";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "my_field:int32 1 =2532323 @([\"obsolete\": true])";
            field = SchemaFileParserV1.ReadField(line);
            Assert.Equal("my_field", field.Name);
            Assert.Equal(1, field.Index);
            Assert.Equal(SchemaTypeFieldValueType.Primitive("int32"), field.ValueType);
            Assert.Equal("2532323", field.DefaultValue);
            Assert.Equal(new Dictionary<string, object> {
                { "obsolete", true },
            }, field.Metadata);
        }

        [Fact]
        public void Test_avoid_field_index_collision()
        {
            var context = ParserContext.CreateContext();
            context.CurrentTypeBuilder = new SchemaTypeBuilder();

            string line = "my_field:string 5";
            var field = SchemaFileParserV1.ReadField(line);
            context.CurrentTypeBuilder.AddField(field);

            line = "another_field:int32 5";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));

            line = "yeah:int32 6";
            field = SchemaFileParserV1.ReadField(line);
            context.CurrentTypeBuilder.AddField(field);

            line = "yeah:int32 7";
            Assert.IsType<InvalidSchemaException>(Record.Exception(() => SchemaFileParserV1.ReadField(line)));
        }
    }
}