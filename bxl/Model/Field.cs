using System;
using BattleAxe;
using System.Collections.Generic;

/// <summary>
/// Based on select * from bxl.TemplateFields
/// </summary>
namespace bxl.Model {
    public class Field : IBattleAxe {
        public object this[string property] {
            get {
                switch (property) {
                    case "TemplateFieldId": return TemplateFieldId;
                    case "TemplateId": return TemplateId;
                    case "FieldName": return FieldName;
                    case "FieldType": return FieldType;
                    case "Created": return Created;
                    case "IsNullable": return IsNullable;
                    case "ColumnIndex":return ColumnIndex;
                    case "ExcelColumnReference": return ExcelColumnReference;
                    case "ColumnLength": return ColumnLength;
                    default:
                        return null;
                }
            }
            set {
                switch (property) {
                    case "TemplateFieldId": this.TemplateFieldId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "TemplateId": this.TemplateId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "FieldName": this.FieldName = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "FieldType": this.FieldType = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Created": this.Created = value is DateTime ? (DateTime)value : value != null ? Convert.ToDateTime(value) : System.DateTime.MinValue; break;
                    case "IsNullable": this.IsNullable = value is bool ? (bool)value : value != null ? Convert.ToBoolean(value) : false; break;
                    case "ColumnIndex": this.ColumnIndex = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "ExcelColumnReference": ExcelColumnReference = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ColumnLength": this.ColumnLength = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    default:
                        break;
                }
            }
        }

        public int TemplateFieldId { get; set; }
        public int TemplateId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public DateTime Created { get; set; }
        public bool IsNullable { get; set; }
        public int ColumnIndex { get; set; }
        public int ColumnLength { get; set; } = 255;
        public bool RequiresFurtherValidation { get; set; } = true;
        public string ExcelColumnReference { get; set; }
        public string DatabaseType {
            get {
                if (this.FieldType.ToLower().Contains(Constants.String)) {
                    return $"nvarchar({this.ColumnLength})";
                }
                else if (FieldType == Constants.DateTime) {
                    return Constants.DateTime;
                }
                else {
                    return "decimal(18,5)";
                }
            }
        }

        string _dataFieldName = null;
        public string DataFieldName {
            get {
                if(_dataFieldName == null) {
                    _dataFieldName = this.FieldName.Replace(" ", "");
                }
                return _dataFieldName;
            }
        }

        public object Value { get; set; }

        public string FailReason { get; set; }

        public void SetValue(string value) {
            if (string.IsNullOrEmpty(value)) {
                this.Value = DBNull.Value;
            }
            else {
                if (this.FieldType.ToLower().Contains(Constants.String)) {
                    Value = value;
                }
                else if (this.FieldType == Constants.DateTime) {
                    Value = DateTime.Parse(value);
                }
                else {
                    Value = decimal.Parse(value);
                }
            }
        }

        public void Update() {
            Data.Field.Update.Execute(this);
        }

        public static List<Field> GetFields(int templateId) =>
            Data.Field.List.ToList(new Field { TemplateId = templateId });
    }
}