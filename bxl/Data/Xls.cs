using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace bxl.Data {
    //this class only need to be concerned with data needs
    public class Xls : DataFileHandler {

        public Xls() { }
        public string GetConnectionString(string fileName, bool withHeaders) {
            var header = withHeaders ? "YES" : "NO";
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=\"Excel 12.0 Xml;HDR={header};IMEX=1;\";";
        }        

        string GetFirstSheetName(string fileName) {
                      
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=\"Excel 12.0 Xml;\"";
            
            using (var objConn = new OleDbConnection(connectionString)) {
                objConn.Open();
                using (var dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null)) {
                    if (dt == null) {
                        return null;
                    }                    
                    foreach (DataRow row in dt.Rows) {
                        return row["TABLE_NAME"].ToString();
                    }
                }
            }
            return null;
        }

        public override List<Model.Field> FieldDefinitions(string fileName, Model.Template template) {
            var fields = new List<Model.Field>();
            string sheetName = GetFirstSheetName(fileName);
            var connectionString = GetConnectionString(fileName, template.HasHeaders);
            using (OleDbConnection SQLConn = new OleDbConnection(connectionString)) {                
                string selectStatement = $"SELECT * FROM [{sheetName}]";
                using (OleDbCommand selectCommand = new OleDbCommand(selectStatement, SQLConn)) {
                    SQLConn.Open();
                    using (var reader = selectCommand.ExecuteReader()) {
                        for (int i = 0; i < reader.FieldCount; i++) {
                            fields.Add(new Model.Field {
                                ColumnIndex = i,
                                FieldName = reader.GetName(i),
                                IsNullable = false,
                                FieldType = TranslatedFieldType(reader.GetFieldType(i).ToString())
                            });
                        }                        
                    }
                }
            }
            return fields;
        }

        internal override void ImportSubhandler(ImportResults import) {
            var fields = new List<Model.Field>();
            string sheetName = GetFirstSheetName(import.ImportFileName);
            var connectionString = GetConnectionString(import.ImportFileName, import.Template.HasHeaders);
            int rowIndex = 1;
            using (OleDbConnection SQLConn = new OleDbConnection(connectionString)) {
                string selectStatement = $"SELECT * FROM [{sheetName}]";
                using (OleDbCommand selectCommand = new OleDbCommand(selectStatement, SQLConn)) {
                    SQLConn.Open();
                    using (var reader = selectCommand.ExecuteReader()) {
                        ReadHeader(fields, reader);
                        if (!import.Template.HasGoodColumnMatch(fields)) {
                            import.Message = $"File formatting issue: file,{import.ImportFileName}, columns do not match the reference file,{import.Template.ReferenceFile}";
                            this.ShouldAttemptInsert = false;
                            return;
                        }
                        while (reader.Read()) {
                            fields.ForEach(f => f.Value = DBNull.Value);
                            foreach (var field in fields) {
                                var value = reader.GetValue(field.ColumnIndex).ToString();
                                SetFieldType(field, value);
                            }
                            this.Insert(fields, import, rowIndex);
                            rowIndex++;
                        }
                    }
                }
            }
        }

        private void ReadHeader(List<Model.Field> fields, OleDbDataReader reader) {
            for (int i = 0; i < reader.FieldCount; i++) {
                fields.Add(new Model.Field {
                    ColumnIndex = i,
                    FieldName = reader.GetName(i),
                    IsNullable = false,
                    FieldType = TranslatedFieldType(reader.GetFieldType(i).ToString())
                });
            }
        }

        string TranslatedFieldType(string fieldType) {
            if(fieldType == "System.String") {
                return Constants.String;
            }
            else if(fieldType =="System.DateTime") {
                return Constants.DateTime;
            }
            else {
                return Constants.Number;
            }
        }

        void SetFieldType(Model.Field field, string value) {
            if (field.FieldType != Constants.String) {                
                if (string.IsNullOrEmpty(value)) {
                    field.IsNullable = true;
                }
                else {                    
                    var type = GetFieldType(value);
                    if (field.FieldType == null) {
                        field.FieldType = type;
                    }
                    else if (type != field.FieldType) {
                        field.FieldType = Constants.String;
                    }
                }                
            }
            field.SetValue(value);
        }
    }
}
