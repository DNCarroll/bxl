using System;
using BattleAxe;
namespace bxl.Model {
    public class Error : IBattleAxe {
        public object this[string property] {
            get {
                switch (property) {
                    case "ErrorId": return ErrorId;
                    case "TemplateId": return TemplateId;
                    case "ColumnIndex": return ColumnIndex;
                    case "RowIndex": return RowIndex;
                    case "ColumnName": return ColumnName;
                    case "Issue": return Issue;
                    case "ImportFileName": return ImportFileName;
                    case "Value":return Value;
                    default:
                        return null;
                }
            }
            set {
                switch (property) {
                    case "ErrorId": this.ErrorId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "TemplateId": this.TemplateId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "ColumnIndex": this.ColumnIndex = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "RowIndex": this.RowIndex = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "ColumnName": this.ColumnName = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Issue": this.Issue = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "ImportFileName": this.ImportFileName = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Value": this.Value = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    default:
                        break;
                }
            }
        }

        public int ErrorId { get; set; }
        public int TemplateId { get; set; }
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public string ColumnName { get; set; }
        public string Issue { get; set; }
        public string ImportFileName { get; set; }
        public string Value { get; set; }
    }
}