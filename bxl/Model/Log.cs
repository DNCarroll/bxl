using System;
using BattleAxe;
/// <summary>
/// Based on select * from bxl.Logs
/// </summary>
namespace bxl.Model {
    public class Log : IBattleAxe {
        public object this[string property] {
            get {
                switch (property) {
                    case "LogId": return LogId;
                    case "TemplateId": return TemplateId;
                    case "RanBy": return RanBy;
                    case "Occurred": return Occurred;
                    case "Results": return Results;
                    case "Notes": return Notes;
                    default:
                        return null;
                }
            }
            set {
                switch (property) {
                    case "LogId": this.LogId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "TemplateId": this.TemplateId = value is int ? (int)value : value != null ? Convert.ToInt32(value) : 0; break;
                    case "RanBy": this.RanBy = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Occurred": this.Occurred = value is DateTime ? (DateTime)value : value != null ? Convert.ToDateTime(value) : System.DateTime.MinValue; break;
                    case "Results": this.Results = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    case "Notes": this.Notes = value is string ? (string)value : value != null ? Convert.ToString(value) : System.String.Empty; break;
                    default:
                        break;
                }
            }
        }

        public int LogId { get; set; }
        public int TemplateId { get; set; }
        public string RanBy { get; set; }
        public DateTime Occurred { get; set; }
        public string Results { get; set; }
        public string Notes { get; set; }

        public void Update() =>
            Data.Log.Update.Execute(this);
    }
}