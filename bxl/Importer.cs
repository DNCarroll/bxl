using BattleAxe;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace bxl {
    public class Import {

        public Model.Template Template { get; set; }
        public bool LogSteps { get; set; }
        public Import(Model.Template template, bool logSteps = false) {
            this.Template = template;
            this.LogSteps = logSteps;
        }

        /// <summary>
        /// will use the pattern recognition of the importer to import all files
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public List<ImportResults> All() {
            var results = GetFilesToImport();
            if (results.Count == 0) {
                results.Add(new ImportResults { Message = "No Files were found to import", Success = true, Template = this.Template });
            }
            return results;
        }

        /// <summary>
        /// Imports only the one file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public ImportResults Single(string fileName) {
            var importResult = new ImportResults() {
                ImportFileName = fileName,
                Template = this.Template
            };
            Execute(importResult);
            return importResult;
        }
                
        private List<ImportResults> GetFilesToImport() {
            var results = new List<ImportResults>();
            if (!string.IsNullOrEmpty(Template.ImportDirectory)) {
                var files = System.IO.Directory.GetFiles(Template.ImportDirectory, Template.FileNamePattern);
                foreach (var file in files) {
                    var importResult = new ImportResults {
                        ImportFileName = file,
                        Template = Template
                    };
                    results.Add(importResult);
                    Execute(importResult);
                }
            }
            return results;
        }
        
        private void Execute(ImportResults importResults) {
            importResults.Template.StartErrorId = 0;
            importResults.Template.EndErrorId = 0;
            importResults.Success =
                Execute(CreateBackupDirectory, importResults, nameof(CreateBackupDirectory)) &&
                Execute(MoveToProcessing, importResults, nameof(MoveToProcessing)) &&
                Execute(CreateWorkingTable, importResults, nameof(CreateWorkingTable)) &&
                Execute(PurgeFromDatabase, importResults, nameof(PurgeFromDatabase)) ?
                    Execute(DataToDatabase, importResults, nameof(DataToDatabase)) ?
                        Execute(RunSubsequentProcedure, importResults, nameof(RunSubsequentProcedure)) :
                        Execute(CleanseFailedImport, importResults, nameof(CleanseFailedImport)) 
                    : false;
            //always email
            //remove the Processing File always
            Execute(DeleteProcessingFile, importResults, nameof(DeleteProcessingFile));
            Execute(SendErrorEmail, importResults, nameof(SendErrorEmail));
        }

        //not ready need to log any mistakes
        private bool Execute(Func<ImportResults, bool> action, ImportResults importResults, string methodName) {
            bool result = false;
            try {
                Stopwatch stopWatch = null;
                if (this.LogSteps) {
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                }
                result = action(importResults);
                if (this.LogSteps) {
                    Data.Log.Update.Execute(new Model.Log {
                        Notes = $"{methodName} run took {stopWatch.ElapsedMilliseconds}ms",
                        Occurred = DateTime.Now,
                        RanBy = "Nathan_Carroll",
                        TemplateId = importResults.Template.TemplateId
                    });
                    stopWatch.Stop();
                }
            }
            catch (Exception ex) {
                //record ex here
                importResults.Message = ex.Message;
                result = false;
            }
            return result;
        }
                
        private bool CreateBackupDirectory(ImportResults importResults) {
            if (this.Template.ImportDirectoryHandling != Model.ImportDirectoryHandling.None) {
                var template = importResults.Template;
                var processingDirectory = Path.Combine(this.Template.ImportDirectory, "Processing");
                template.BackupDirectory = System.IO.Path.Combine(template.ImportDirectory, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
                if (!Directory.Exists(template.BackupDirectory)) {
                    Directory.CreateDirectory(template.BackupDirectory);
                }
                if (!Directory.Exists(processingDirectory)) {
                    Directory.CreateDirectory(processingDirectory);
                }
                return Directory.Exists(template.BackupDirectory) && Directory.Exists(processingDirectory);
            }
            return true;
        }
                
        private bool MoveToProcessing(ImportResults importResults) {
            if (this.Template.ImportDirectoryHandling != Model.ImportDirectoryHandling.None) {
                var fileInfo = new FileInfo(importResults.ImportFileName);
                var datePart = DateTime.Now.ToString(Constants.DateFormattingForFileName);
                var backUpFile = Path.Combine(importResults.Template.BackupDirectory, $"{fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."))}{datePart}{fileInfo.Extension}");
                var processingDirectory = Path.Combine(this.Template.ImportDirectory, "Processing");
                var newPath = Path.Combine(processingDirectory, fileInfo.Name);
                if (File.Exists(newPath)) {
                    File.Delete(newPath);
                }
                File.Move(importResults.ImportFileName, newPath);
                if (File.Exists(newPath)) {
                    importResults.ImportFileName = newPath;
                    if (this.Template.ImportDirectoryHandling == Model.ImportDirectoryHandling.Archive) {
                        File.Copy(importResults.ImportFileName, backUpFile, true);
                    }
                }
                return File.Exists(newPath);
            }
            return true;
        }
                
        private bool CreateWorkingTable(ImportResults importResults) {
            if (!string.IsNullOrEmpty(importResults.Template.LoadTransformProcedure)) {
                using (var conn = new SqlConnection(importResults.Template.ConnectionString)) {
                    using (var cmd = new SqlCommand(importResults.Template.CreateTableComamndStatement(), conn)) {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }
                
        private bool PurgeFromDatabase(ImportResults importResults) {
            if (importResults.Template.PurgeWorkingOnEveryRun) {
                using(var conn = new SqlConnection(importResults.Template.ConnectionString)) {
                    using (var cmd = new SqlCommand($"TRUNCATE TABLE bxl.{importResults.Template.WorkingTableName}", conn)) {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }               
        
        private bool DataToDatabase(ImportResults importResults) {
            var handler = Data.DataFileHandler.GetHandler(importResults.ImportFileName);
            return handler.Import(importResults);
        }
                
        private bool RunSubsequentProcedure(ImportResults importResults) {
            if (!string.IsNullOrEmpty(importResults.Template.LoadTransformProcedure)) {
                using (var conn = new SqlConnection(importResults.Template.ConnectionString)) {
                    using (var cmd = new SqlCommand(importResults.Template.LoadTransformProcedure, conn)) {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }
         
        private bool CleanseFailedImport(ImportResults importResults) {
            if (importResults.Template.PurgeWorkingOnEveryRun) {
                using (var conn = new SqlConnection(importResults.Template.ConnectionString)) {
                    using (var cmd = new SqlCommand($"DELETE FROM bxl.{importResults.Template.WorkingTableName} WHERE ImportFileName = @ImportFileName AND ImportDate = @ImportDate", conn)) {
                        cmd.Parameters.AddWithValue("@ImportFileName", importResults.ImportFileName);
                        cmd.Parameters.AddWithValue("@ImportDate", importResults.ImportDate);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        private bool DeleteProcessingFile(ImportResults importResults) {
            if (this.Template.ImportDirectoryHandling != Model.ImportDirectoryHandling.None) {
                System.IO.File.Delete(importResults.ImportFileName);
                return true;
            }
            return true;
        }

        private bool SendErrorEmail(ImportResults importResults) {
            if (!string.IsNullOrEmpty(Template.OnErrorEmail) && Template.StartErrorId > 0) {
                Template.Body = $@"Processing of {Template.ProjectGroup}: {Template.ProcessName} for the import file, {importResults.ShortName}, has failed with data issues.<br><br>The data issues are in attached file.";
                Template.Subject = "Data import error";
                Data.Template.EmailOnError.Execute(Template);
            }
            return true;
        }

    }

    public class ImportResults {
        public string Message { get; set; }

        private string importFileName;

        public string ImportFileName {
            get { return importFileName; }
            set {
                importFileName = value;
                shortName = value.Substring(value.LastIndexOf("\\") + 1);
            }
        }

        private string shortName;
        public string ShortName {
            get {
                return shortName;
            }
        }

        public DateTime ImportDate { get; set; } = DateTime.Now;
        public bool Success { get; set; } = false;
        public Model.Template Template { get; set; }
    }
}
