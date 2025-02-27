using System.IO;
using NUnit.Framework;

namespace ParquetSharp.Test
{
    [TestFixture]
    public class TestNestedReads
    {

        /// <summary>
        /// Currently ParquetSharp cannot write nested structures.
        /// We are using a Parquet file found in TestFiles/nested.parquet, generated by TestFiles/generate_parquet.py to test the ParquetSharp reader.
        /// </summary>
        [Test]
        public void CanReadNestedStructure()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory!, "TestFiles/nested.parquet");

            using var fileReader = new ParquetFileReader(path);
            using var rowGroupReader = fileReader.RowGroup(0);

            // first_level_long
            using var column0Reader = rowGroupReader.Column(0).LogicalReader<long?>();
            var column0Actual = column0Reader.ReadAll(2);
            var column0Expected = new[] {1, 2};
            Assert.AreEqual(column0Expected, column0Actual);

            // first_level_nullable_string
            using var column1Reader = rowGroupReader.Column(1).LogicalReader<string?>();
            var column1Actual = column1Reader.ReadAll(2);
            var column1Expected = new[] {null, "Not Null String"};
            Assert.AreEqual(column1Expected, column1Actual);

            // nullable_struct.nullable_struct_string
            using var column2Reader = rowGroupReader.Column(2).LogicalReader<string?>();
            var column2Actual = column2Reader.ReadAll(2);
            var column2Expected = new[] {"Nullable Struct String", null};
            Assert.AreEqual(column2Expected, column2Actual);

            // struct.struct_string
            using var column3Reader = rowGroupReader.Column(3).LogicalReader<string>();
            var column3Actual = column3Reader.ReadAll(2);
            var column3Expected = new[] {"First Struct String", "Second Struct String"};
            Assert.AreEqual(column3Expected, column3Actual);

            // struct_array.array_in_struct_array
            using var column4Reader = rowGroupReader.Column(4).LogicalReader<long?[]?[]>();
            var column4Actual = column4Reader.ReadAll(2);
            var column4Expected = new[] {new[] {new[] {111, 112, 113}, new[] {121, 122, 123}}, new[] {new[] {211, 212, 213}}};
            Assert.AreEqual(column4Expected, column4Actual);

            // struct_array.string_in_struct_array
            using var column5Reader = rowGroupReader.Column(5).LogicalReader<string[]>();
            var column5Actual = column5Reader.ReadAll(2);
            var column5Expected = new[] {new[] {"First String", "Second String"}, new[] {"Third String"}};
            Assert.AreEqual(column5Expected, column5Actual);
        }
    }
}
