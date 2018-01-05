using NUnit.Framework;
using System.IO;

namespace bxl.ConstructorTesting {

    [TestFixture]
    public class Construct {

        [Test]
        public void Xlsx() => Assert(GetValidation("TestXLSXFile.xlsx"));

        [Test]
        public void Xls() => Assert(GetValidation("TestXLSXFile.xls"));

        [Test]
        public void Csv() => Assert(GetValidation("TestXLSXFile.csv"));

        [Test]
        public void Txt() => Assert(GetValidation("TestXLSXFile.txt"));

        Model.Template GetValidation(string fileName) {
            return bxl.Construct.Template(
                    new Model.Template {
                        HasHeaders = true,
                        ProcessName = "DoesntMatter",
                        ProjectGroup = "DoesntMatter"
                    },
                    GetFile(fileName)
            );
        }

        string GetFile(string fileName) => Path.Combine(Helper.ExecuteablePath(), fileName);

        private void Assert(Model.Template validation) {
            NUnit.Framework.Assert.IsTrue(validation.Fields.Count == 6 &&
                          validation.Fields[0].FieldType == Constants.Number &&
                          validation.Fields[1].FieldType == Constants.DateTime &&
                          validation.Fields[2].FieldType == Constants.Number &&
                          validation.Fields[3].FieldType == Constants.String &&
                          validation.Fields[4].FieldType == Constants.String &&
                          validation.Fields[5].FieldType == Constants.String);
        }

    }
}
