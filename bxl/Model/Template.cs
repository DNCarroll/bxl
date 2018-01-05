using System;
using BattleAxe;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
/// <summary>
/// Based on SELECT * FROM bxl.Templates
/// </summary>
namespace bxl.Model {
    public enum ImportDirectoryHandling {
        None,
        Archive,
        Delete
    }

    public enum RowBehaviorOnFailure {
        Break,
        Continue
    }

    public class Template : IBattleAxe {
        public object this[string property] {
            get {
                switch (property) {
                    case "TemplateId": return TemplateId;
                    case "PurgeWorkingOnEveryRun": return PurgeWorkingOnEveryRun;
                    case "ProcessName": return ProcessName;
                    case "ProjectGroup": return ProjectGroup;
                    case "IsActive": return IsActive;
                    case "ConnectionString": return ConnectionString;
                    case "LoadTransformProcedure": return LoadTransformProcedure;
                    case "FileNamePattern": return FileNamePattern;
                    case "OnErrorEmail": return OnErrorEmail;
                    case "Created": return Created;
                    case "HasHeaders": return HasHeaders;
                    case "Delimiter": return Delimiter;
                    case "ImportDirectory": return ImportDirectory;
                    case "ReferenceFile": return ReferenceFile;
                    case "ErrorsSharePath": return ErrorsSharePath;
                    case "EmailProfile": return EmailProfile;
                    case "Subject": return Subject;
                    case "Body":return Body;
                    case "EndErrorId": return EndErrorId;
                    case "StartErrorId": return StartErrorId;
                    case "ImportDirectoryHandling": return ImportDirectoryHandling;
                    case "RowBehaviorOnFailure": return RowBehaviorOnFailure;
                    default:
                        return null;
                }
            }
            set {
                switch (property) {
                    case "ReferenceFile": this.ReferenceFile = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "TemplateId": this.TemplateId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "StartErrorId": this.StartErrorId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "EndErrorId": this.EndErrorId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "PurgeWorkingOnEveryRun": this.PurgeWorkingOnEveryRun = value is bool ? (bool)value : value != null ? Convert.ToBoolean(value) : false; break;
                    case "ProcessName": this.ProcessName = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ProjectGroup": this.ProjectGroup = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "IsActive": this.IsActive = value is bool ? (bool)value : value != null ? Convert.ToBoolean(value) : false; break;
                    case "ConnectionString": this.ConnectionString = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "LoadTransformProcedure": this.LoadTransformProcedure = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "FileNamePattern": this.FileNamePattern = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "OnErrorEmail": this.OnErrorEmail = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Created": this.Created = value is DateTime ? (DateTime)value : value != null ? Convert.ToDateTime(value) : System.DateTime.MinValue; break;
                    case "HasHeaders": this.HasHeaders = value is bool ? (bool)value : value != null ? Convert.ToBoolean(value) : false; break;
                    case "Delimiter": this.Delimiter = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ImportDirectory": this.ImportDirectory = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ErrorsSharePath": this.ErrorsSharePath = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "EmailProfile": this.EmailProfile = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Subject": this.Subject = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Body": this.Body = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ImportDirectoryHandling": this.ImportDirectoryHandling = value ==null ?default(ImportDirectoryHandling) : (ImportDirectoryHandling)System.Enum.Parse(typeof(ImportDirectoryHandling), value.ToString()); break;
                    case "RowBehaviorOnFailure": this.RowBehaviorOnFailure = value == null ? default(RowBehaviorOnFailure) : (RowBehaviorOnFailure)System.Enum.Parse(typeof(RowBehaviorOnFailure), value.ToString()); break;
                    default:
                        break;
                }
            }
        }
        public RowBehaviorOnFailure RowBehaviorOnFailure { get; set; }
        public ImportDirectoryHandling ImportDirectoryHandling { get; set; }
        public int StartErrorId { get; set; }
        public int EndErrorId { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string EmailProfile { get; set; }
        public string ErrorsSharePath { get; set; }
        public int TemplateId { get; set; }
        public bool PurgeWorkingOnEveryRun { get; set; }
        public string ProcessName { get; set; }
        public string ProjectGroup { get; set; }
        public bool IsActive { get; set; }
        public string ConnectionString { get; set; }
        public string LoadTransformProcedure { get; set; }
        public string FileNamePattern { get; set; }
        public string OnErrorEmail { get; set; }
        public DateTime Created { get; set; }
        public bool HasHeaders { get; set; }
        public string Delimiter { get; set; }
        public string ImportDirectory { get; set; }
        public string BackupDirectory { get; set; }
        public string ReferenceFile { get; set; }
        public static List<Template> GetByProcessGroup(string processGroup) =>
            Data.Template.List.ToList(new Template { ProjectGroup = processGroup });

        public void Update() {
            Data.Template.Update.Execute(this);
            this.Fields.ForEach(f => {
                f.TemplateId = this.TemplateId;
                f.Update();
            });
        }

        private List<Field> fields;
        [JsonIgnore]
        public List<Field> Fields {
            get {
                if (fields == null) {
                    fields = Data.Field.List.ToList(new Field { TemplateId = this.TemplateId });
                }
                return fields;
            }
            set { fields = value; }
        }

        public void AddField(Field field) {
            field.TemplateId = this.TemplateId;
            field.Update();
        }

        public string WorkingTableName {
            get {

                return $"{ProjectGroup}_{ProcessName}_{TemplateId}";
            }
        }

        SqlCommand _insertCommand = null;
        internal SqlCommand GetInsertCommand() {
            if (_insertCommand == null) {
                this._insertCommand = new SqlCommand(InsertStatement());
                this._insertCommand.Parameters.Add("@ImportFileName", SqlDbType.NVarChar, 500);
                this._insertCommand.Parameters.Add("@ImportDate", SqlDbType.DateTime);
                foreach (var field in this.Fields) {
                    var parameter = new SqlParameter {
                        ParameterName = $"@{field.DataFieldName}",
                        SourceColumn = field.FieldName
                    };
                    if (field.FieldType.ToLower().Contains(Constants.String)) {
                        parameter.SqlDbType = SqlDbType.NVarChar;
                        parameter.Size = field.ColumnLength;
                    }
                    else if (field.FieldType == Constants.DateTime) {
                        parameter.SqlDbType = SqlDbType.DateTime;
                    }
                    else {
                        parameter.SqlDbType = SqlDbType.Decimal;
                        parameter.Precision = 18;
                        parameter.Scale = 5;
                    }
                    this._insertCommand.Parameters.Add(parameter);
                }
            }
            return _insertCommand;
        }

        string InsertStatement() {

            var insertCommand = $@"INSERT INTO bxl.{WorkingTableName}
                                        (
                                            ImportFileName,
                                            ImportDate,
                                            {string.Join(",", this.Fields.Select(f => $"{f.DataFieldName}").ToArray())}
                                        )
                                        VALUES
                                        (
                                            @ImportFileName,
                                            @ImportDate,
                                            {string.Join(",", this.Fields.Select(f => $"@{f.DataFieldName}").ToArray())}
                                        );";


            return insertCommand;

        }

        public string CreateTableComamndStatement() {
            var fields = Fields.Select(f => $"[{f.DataFieldName}] {f.DatabaseType}").ToArray();
            var createTable =
            $@"IF NOT EXISTS(
                    SELECT 1 FROM sys.objects  WHERE name = '{WorkingTableName}' AND type = 'U' AND SCHEMA_NAME(schema_id) = 'bxl') BEGIN
                    CREATE TABLE bxl.{WorkingTableName}
                    (
                        {string.Join(", ", fields)},
                        ImportFileName nvarchar(500),
			            ImportDate datetime
		            );
            END";
            return createTable;
        }

        public int CurrentTableCount() {
            var ret = 0;
            try {
                var selectCountCommand =
                $@"IF EXISTS(
                    SELECT 1 FROM sys.objects  WHERE name = '{WorkingTableName}' AND type = 'U' AND SCHEMA_NAME(schema_id) = 'bxl') BEGIN
                    SELECT COUNT(1) countOf FROM bxl.{WorkingTableName};
                END";
                using (var conn = new SqlConnection(this.ConnectionString)) {
                    using (var cmd = new SqlCommand(selectCountCommand, conn)) {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader()) {
                            while (reader.Read()) {
                                ret = reader.GetInt32(0);
                                break;
                            }
                        }
                    }
                }
            }
            catch {

            }
            return ret;
        }

        public void CheckFieldTypes(List<Model.Field> fields, int rowIndex, string fileName) {
            for (int i = 0; i < Fields.Count; i++) {
                Error error = null;
                var referenceField = Fields[i];
                var incomingField = HasHeaders ?
                                        fields.FirstOrDefault(f => f.FieldName == referenceField.FieldName) :
                                        fields[i];
                if (incomingField != null) {
                    if (referenceField.FieldType == Constants.String || incomingField.Value == DBNull.Value) {
                        continue;
                    }
                    var type = Data.DataFileHandler.GetFieldType(incomingField.Value.ToString());
                    if (type != referenceField.FieldType) {
                        error = new Error {
                            RowIndex = rowIndex,
                            ColumnIndex = referenceField.ColumnIndex + 1,
                            ColumnName = referenceField.FieldName,
                            Issue = $"Expected data type: {referenceField.FieldType}",
                            Value = incomingField.Value.ToString(),
                            ImportFileName = fileName
                        };
                    }
                }
                else {
                    error =
                        new Error {
                            RowIndex = rowIndex,
                            ColumnIndex = referenceField.ColumnIndex + 1,
                            ColumnName = referenceField.FieldName,
                            Issue = $"The source file did not contain a value",
                            ImportFileName = fileName
                        };
                }
                if (error != null) {
                    WriteError(error);
                }
            }
        }
        
        void WriteError(Error error) {
            error.TemplateId = this.TemplateId;
            error.Execute(Data.Error.Insert);
            this.StartErrorId = this.StartErrorId == 0 ? error.ErrorId : this.StartErrorId;
            this.EndErrorId = error.ErrorId;
        }

        public bool HasGoodColumnMatch(List<Model.Field> fields) {
            if (this.HasHeaders) {
                foreach (var field in this.Fields) {
                    var found = fields.FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (found == null) {
                        return false;
                    }
                }
                return true;
            }
            else {
                return Fields.Count != fields.Count;
            }
        }
    }
}