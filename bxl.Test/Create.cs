using NUnit.Framework;

namespace bxl.DatabasesCreate {
    [TestFixture]
    public class Create {

        [Test]
        public void Execute() {
            Helper.SetConnectionString();
            var path = Helper.ExecuteablePath();
            var file = System.IO.Path.Combine(path, "TestXLSXFile.xlsx");
            var validation = bxl.Construct.Template(
                    new Model.Template {
                        OnErrorEmail = "nathan.carroll@?.com",
                        HasHeaders = true,
                        ProcessName = "Xlsx",
                        ProjectGroup = "Test",
                        ReferenceFile = file,
                        ConnectionString = Data.Connection.Value,
                        ImportDirectory = System.IO.Path.Combine(Helper.ExecuteablePath(), "Xlsx"),
                        IsActive = true,
                        LoadTransformProcedure = "bxl.LoadOpenXmlFile",
                        PurgeWorkingOnEveryRun = true
                    },
                    file
            );
            validation.Update();
            file = System.IO.Path.Combine(path, "TestXLSXFile.xls");
            validation = bxl.Construct.Template(
                    new Model.Template {
                        OnErrorEmail = "nathan.carroll@?.com",
                        HasHeaders = true,
                        ProcessName = "Xls",
                        ProjectGroup = "Teset",
                        ReferenceFile = file,
                        ConnectionString = Data.Connection.Value,
                        ImportDirectory = System.IO.Path.Combine(Helper.ExecuteablePath(), "Xls"),
                        IsActive = true,
                        LoadTransformProcedure = "bxl.LoadXlsFile",
                        PurgeWorkingOnEveryRun = true
                    },
                    file
            );
            validation.Update();
            file = System.IO.Path.Combine(path, "TestXLSXFile.csv");
            validation = bxl.Construct.Template(
                    new Model.Template {
                        OnErrorEmail = "nathan.carroll@?.com",
                        HasHeaders = true,
                        ProcessName = "Csv",
                        ProjectGroup = "Test",
                        ReferenceFile = file,
                        ConnectionString = Data.Connection.Value,
                        ImportDirectory = System.IO.Path.Combine(Helper.ExecuteablePath(), "csv"),
                        IsActive = true,
                        LoadTransformProcedure = "bxl.LoadCsvFile",
                        PurgeWorkingOnEveryRun = true
                    },
                    file
            );
            validation.Update();
            file = System.IO.Path.Combine(path, "TestXLSXFile.txt");
            validation = bxl.Construct.Template(
                    new Model.Template {
                        OnErrorEmail ="nathan.carroll@?.com",
                        HasHeaders = true,
                        ProcessName = "Txt",
                        ProjectGroup = "Test",
                        ReferenceFile = file,
                        ConnectionString = Data.Connection.Value,
                        ImportDirectory = System.IO.Path.Combine(Helper.ExecuteablePath(), "txt"),
                        IsActive = true,
                        LoadTransformProcedure = "bxl.LoadTxtFile",
                        PurgeWorkingOnEveryRun = true
                    },
                    file
            );
            validation.Update();
        }

    }
}
