using NUnit.Framework;
using System.Linq;

namespace bxl.FilterTesting {

    public enum TestType {
        LooseNameLooseExtension,
        LooseNameTightExtension,
        TightNameLooseExtension,
        TightNameTightExtension
    }

    public abstract class FileFilterTestBase {
        
        private Model.Template template;
        public Model.Template Template {
            get {
                if (template == null) {
                    Helper.SetConnectionString();
                    template = Model.Template.GetByProcessGroup("Test").FirstOrDefault();                 
                }
                switch (this.TestType) {
                    case TestType.LooseNameLooseExtension:
                        template.FileNamePattern = "*";
                        ExpectedFileCount = 6;
                        break;
                    case TestType.TightNameLooseExtension:
                        template.FileNamePattern = "TestXLSXFile.*";
                        ExpectedFileCount = 4;
                        break;
                    case TestType.TightNameTightExtension:
                        template.FileNamePattern = "TestXLSXFile.xlsx";
                        ExpectedFileCount = 1;
                        break;
                    case TestType.LooseNameTightExtension:
                        ExpectedFileCount = 2;
                        template.FileNamePattern = "*.xlsx";
                        break;
                    default:
                        break;
                }
                template.ImportDirectory = template.ImportDirectory.Replace("Xlsx", "TestingDirectoryFiltering");
                return template;
            }
            set { template = value; }
        }
        public TestType TestType{ get; set; }
        public int ExpectedFileCount { get; set; }

     
        public void Test() {
            var files = System.IO.Directory.GetFiles(Template.ImportDirectory, Template.FileNamePattern);
            Assert.IsTrue(files.Length == this.ExpectedFileCount);
        }
    }
    
    [TestFixture]
    public class Filter : FileFilterTestBase {
        [Test]
        public void Loose_name_tight_extension() {
            this.TestType = TestType.LooseNameTightExtension;
            this.Test();
        }

        [Test]
        public void Tight_name_tight_extension() {
            this.TestType = TestType.TightNameTightExtension;
            this.Test();
        }

        [Test]
        public void Loose_name_loose_extension() {
            this.TestType = TestType.LooseNameLooseExtension;
            this.Test();
        }

        [Test]
        public void Tight_name_loose_extension() {
            this.TestType = TestType.TightNameLooseExtension;
            this.Test();
        }
    }
}
