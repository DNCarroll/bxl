using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace bxl.Data {
    public interface IDataFileHandler {
        List<Model.Field> FieldDefinitions(string fileName, Model.Template template);
        bool Import(ImportResults import);
    }

    public abstract class DataFileHandler : IDataFileHandler {

        public static IDataFileHandler GetHandler(string referenceFile) {
            IDataFileHandler dataHandler = null;
            var fileInfo = new FileInfo(referenceFile);
            var extension = fileInfo.Extension.ToLower();
            if (extension == ".xlsx") {
                dataHandler = new bxl.Data.Xlsx();
            }
            else if (extension == ".xls") {
                dataHandler = new bxl.Data.Xls();
            }
            else {
                dataHandler = new bxl.Data.Txt();
            }
            return dataHandler;
        }
        public abstract List<Model.Field> FieldDefinitions(string fileName, Model.Template template);

        public bool Import(ImportResults import) {
            this.ImportCount = 0;
            this.ShouldAttemptInsert = true;
            this.ImportSubhandler(import);
            return this.ShouldAttemptInsert;            
        }

        internal abstract void ImportSubhandler(ImportResults import);

        public int ImportCount { get; set; }
        public bool ShouldAttemptInsert { get; set; }

        public void Insert(List<Model.Field> fields, ImportResults import, int rowIndex) {
            import.Template.CheckFieldTypes(fields, rowIndex, import.ShortName);
            this.ShouldAttemptInsert = import.Template.RowBehaviorOnFailure == Model.RowBehaviorOnFailure.Continue ||
                    (import.Template.RowBehaviorOnFailure == Model.RowBehaviorOnFailure.Break && import.Template.StartErrorId == 0);
            if (this.ShouldAttemptInsert) {
                try {
                    this.Insert(import, fields);
                }
                catch (Exception ex) {
                    //import.SuccessfulImportRun = false;
                    import.Message = ex.Message;
                    this.ShouldAttemptInsert = import.Template.RowBehaviorOnFailure == Model.RowBehaviorOnFailure.Continue;
                }
            }
        }

        void Insert(ImportResults import, List<Model.Field> fields) {
            var template = import.Template;
            using (SqlConnection conn = new SqlConnection(template.ConnectionString)) {
                var cmd = template.GetInsertCommand();
                cmd.Connection = conn;
                cmd.Parameters["@ImportFileName"].Value = import.ImportFileName;
                cmd.Parameters["@ImportDate"].Value = import.ImportDate;
                foreach (var field in template.Fields) {
                    var found = fields.FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (found != null) {
                        if (found.FieldType == field.FieldType) {
                            cmd.Parameters["@" + field.DataFieldName].Value = found.Value;
                        }
                        else {
                            field.SetValue(found.Value.ToString());
                            cmd.Parameters["@" + field.DataFieldName].Value = field.Value;
                        }
                    }
                    else {
                        cmd.Parameters["@" + field.DataFieldName].Value = DBNull.Value;
                    }
                }
                conn.Open();
                cmd.ExecuteNonQuery();
                this.ImportCount = this.ImportCount + 1;
            }
        }

        public static string GetFieldType(string value) {
            if (value == null) {
                return null;
            }
            if (double.TryParse(value, out double numberTest)) {
                return Constants.Number;
            }
            else if (DateTime.TryParse(value, out DateTime dateTest)) {
                return Constants.DateTime;
            }
            return Constants.String;
        }

    }
}
