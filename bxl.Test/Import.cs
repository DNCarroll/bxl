using NUnit.Framework;
using System.IO;
using System.Linq;

namespace bxl.ImportTesting {


    public abstract class ImportTests {
        public Model.Template Validation() {
            Helper.SetConnectionString();
            var temp = Model.Template.GetByProcessGroup("Test");
            return temp.FirstOrDefault(v => v.FileNamePattern.EndsWith(this.ExtensionToTest));
        }

        public string ExtensionToTest { get; set; }
        public int ExpectedCount { get; set; }
        public string AlternateImportDirectory { get; set; }
        public Model.RowBehaviorOnFailure RowBehavior { get; set; } = Model.RowBehaviorOnFailure.Break;

        public void Test() {
            var validation = this.Validation();
            var importer = new bxl.Import(validation);
            validation.RowBehaviorOnFailure = this.RowBehavior;
            validation.ImportDirectory = this.AlternateImportDirectory ?? validation.ImportDirectory;
            foreach (var file in Directory.GetFiles(validation.ImportDirectory)) {
                System.IO.File.Delete(file);
            }
            var seedDirectory = Path.Combine(validation.ImportDirectory, "Seed");
            foreach (var file in Directory.GetFiles(seedDirectory, validation.FileNamePattern)) {
                System.IO.File.Copy(file, validation.ImportDirectory + file.Substring(file.LastIndexOf("\\")));
            }

            importer.All();
            var count = validation.CurrentTableCount();
            Assert.IsTrue(count == this.ExpectedCount);
        }
    }

    [TestFixture]
    public class Import_with_RowBehaviorOnFailure_Contintue : ImportTests {
        [Test]
        public void xlsx_should_pass() {
            this.RowBehavior = Model.RowBehaviorOnFailure.Continue;
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "XlxsShouldFail");
            this.ExtensionToTest = ".xlsx";
            this.ExpectedCount = 320;
            this.Test();
        }

        [Test]
        public void xls_should_pass() {
            this.RowBehavior = Model.RowBehaviorOnFailure.Continue;
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "XlsShouldFail");
            this.ExtensionToTest = ".xls";
            this.ExpectedCount = 320;
            this.Test();
        }

        [Test]
        public void csv_should_pass() {
            this.RowBehavior = Model.RowBehaviorOnFailure.Continue;
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "CsvShouldFail");
            this.ExtensionToTest = ".csv";
            this.ExpectedCount = 320;
            this.Test();
        }

        [Test]
        public void txt_should_pass() {
            this.RowBehavior = Model.RowBehaviorOnFailure.Continue;
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "TxtShouldFail");
            this.ExtensionToTest = ".txt";
            this.ExpectedCount = 320;
            this.Test();
        }
    }

    [TestFixture]
    public class Import_with_RowBehaviorOnFailure_Break_without_errors : ImportTests {
        
        [Test]
        public void xlsx_should_pass() {
            this.ExtensionToTest = ".xlsx";
            this.ExpectedCount = 327;
            this.Test();
        }

        [Test]
        public void xls_should_pass() {
            this.ExtensionToTest = ".xls";
            this.ExpectedCount = 327;
            this.Test();
        }

        [Test]
        public void csv_should_pass() {
            this.ExtensionToTest = ".csv";
            this.ExpectedCount = 327;
            this.Test();
        }

        [Test]
        public void txt_should_pass() {
            this.ExtensionToTest = ".txt";
            this.ExpectedCount = 327;
            this.Test();
        }
    }

    [TestFixture]
    public class Import_with_RowBehaviorOnFailure_Break_with_errors_of : ImportTests {
        [Test]
        public void xlsx_should_pass() {
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "XlxsShouldFail");
            this.ExtensionToTest = ".xlsx";
            this.ExpectedCount = 0;
            this.Test();
        }

        [Test]
        public void xls_should_pass() {
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "XlsShouldFail");
            this.ExtensionToTest = ".xls";
            this.ExpectedCount = 0;
            this.Test();
        }

        [Test]
        public void csv_should_pass() {
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "CsvShouldFail");
            this.ExtensionToTest = ".csv";
            this.ExpectedCount = 0;
            this.Test();
        }

        [Test]
        public void txt_should_pass() {
            this.AlternateImportDirectory = Path.Combine(Helper.ExecuteablePath(), "TxtShouldFail");
            this.ExtensionToTest = ".txt";
            this.ExpectedCount = 0;
            this.Test();
        }
    }
}
