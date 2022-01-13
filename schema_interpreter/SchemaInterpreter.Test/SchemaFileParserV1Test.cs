using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Parser;
using SchemaInterpreter.Parser.Builder;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Parser.V1;
using System;
using System.Collections.Generic;
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

        [Fact]
        public void Test_full_parser() 
        {
            static ParserContext GetParserContext(bool addPackage = true)
            {
                var context = ParserContext.CreateContext();
                if(addPackage)
                    context.CurrentPackage = new SchemaPackage("package1", 1);
                
                return context;
            }

            var context = GetParserContext();

            var parser = new SchemaFileParserV1();
            var type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("package1")
                .WriteType(new SchemaType("MyModel", "package2", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("string_value", 0, SchemaTypeFieldValueType.Parse("string"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package2").Wait();

            Assert.Equal("package2", context.CurrentPackage.Name);
            AssertExt.NotThrows(() => context.VerifyImports());

            context = GetParserContext();

            type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("package12")
                .WriteType(new SchemaType("MyModel", "package3", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("string_value", 0, SchemaTypeFieldValueType.Parse("string"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package3").Wait();

            Assert.Equal("package3", context.CurrentPackage.Name);
            AssertExt.Throws<InvalidSchemaException>(() => context.VerifyImports());

            context = GetParserContext();
            
            type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("package1")
                .WriteType(new SchemaType("MyModel", "package4", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("imported_value", 0, SchemaTypeFieldValueType.Custom("package1.NonExistingModel"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package4").Wait();

            Assert.Equal("package4", context.CurrentPackage.Name);
            AssertExt.NotThrows(() => context.VerifyImports());
            AssertExt.Throws<InvalidSchemaException>(() => context.VerifyAllTypes());

            // add a type to core package to test
            context = GetParserContext();
            context.CurrentPackage.AddType(new SchemaType("MyPackage1Model", "package1", null, Array.Empty<SchemaTypeField>()));

            type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("package1")
                .WriteType(new SchemaType("MyModel", "package5", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("imported_value", 0, SchemaTypeFieldValueType.Custom("package1.MyPackage1Model"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package5").Wait();

            Assert.Equal("package5", context.CurrentPackage.Name);
            AssertExt.NotThrows(() => context.VerifyImports());
            AssertExt.NotThrows(() => context.VerifyAllTypes());

            // add a type to package under a directory to test
            context = GetParserContext(addPackage: false);
            context.CurrentPackage = new SchemaPackage("utils/package1", 1);
            context.CurrentPackage.AddType(new SchemaType("MyPackage1Model", "utils/package1", null, Array.Empty<SchemaTypeField>()));

            type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("package1")
                .WriteType(new SchemaType("MyModel", "package6", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("imported_value", 0, SchemaTypeFieldValueType.Custom("package1.MyPackage1Model"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package6").Wait();

            Assert.Equal("package6", context.CurrentPackage.Name);
            AssertExt.Throws<InvalidSchemaException>(() => context.VerifyImports());
            // verifyAllTypes is not called because if verifyImports fails, it not needed to run other verifications.
            
            // add a type to package under a directory to test
            context = GetParserContext(addPackage: false);
            context.CurrentPackage = new SchemaPackage("utils/package1", 1);
            context.CurrentPackage.AddType(new SchemaType("MyPackage1Model", "utils/package1", null, Array.Empty<SchemaTypeField>()));

            type = TestUtils
                .GetBuilder()
                .WriteVersion(1)
                .WriteImport("utils/package1")
                .WriteType(new SchemaType("MyModel", "package7", null, new SchemaTypeField[]
                {
                    new SchemaTypeField("imported_value", 0, SchemaTypeFieldValueType.Custom("package1.MyPackage1Model"), null, false, null, null)
                }))
                .BuildReader();
            parser.ParseFile(type, "package7").Wait();

            Assert.Equal("package7", context.CurrentPackage.Name);
            AssertExt.NotThrows(() => context.VerifyImports());
            AssertExt.NotThrows(() => context.VerifyAllTypes());
        }
    }
}