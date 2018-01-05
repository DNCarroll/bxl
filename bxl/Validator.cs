using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace bxl {
   
    //make this as a web service that EOL, PRT, or OCB could talk to?
    public class Validator {
        public string File { get; set; }
        public Model.Template Template { get; set; }
        public Model.Template IncomingFile { get; set; }

        public Validator(Model.Template template) {
            this.Template = template;

        }
        public Validator(string file, Model.Template template) {
            this.File = file;
            this.Template = template;
            var fi = new FileInfo(file);
            //reqo
            if (template.FileNamePattern != "*") {
                IsValid = Regex.IsMatch(file, template.FileNamePattern);
            }
            else {
                IsValid = true;
            }
            if (!IsValid) {
                Messages.Add("Wrong file extension for the import.");
            }
        }
        public void Execute() {
            if (IsValid) {
                this.IncomingFile = Construct.Template(Template, File);
                this.IsValid = this.Template.HasGoodColumnMatch(this.IncomingFile.Fields);
                if (this.IsValid) {
                    EvaluateFields(this.IncomingFile.Fields);
                }
                else {
                    this.Messages.Add($"Incoming file does not have the correct,{this.Template.Fields.Count}, column but rather {this.IncomingFile.Fields.Count}.");
                }
            }
        }

        private void EvaluateFields(List<Model.Field> fields) {
            for (int i = 0; i < this.Template.Fields.Count; i++) {
                var referenceField = this.Template.Fields[i];
                var incomingField = this.Template.HasHeaders ?
                                        fields.FirstOrDefault(f => f.FieldName == referenceField.FieldName) :
                                        fields[i];
                EvaluateField(referenceField, incomingField);
            }
        }

        private void EvaluateField(Model.Field referenceField, Model.Field incomingField) {
            if (incomingField != null) {
                if (incomingField.FieldType != referenceField.FieldType) {
                    this.Messages.Add($"Template shows {referenceField.FieldType} type for {referenceField.FieldName} but incoming file shows {incomingField.FieldType} type");
                    this.IsValid = false;
                }
            }
            else {
                this.Messages.Add($"Template shows not matching field for {referenceField.FieldName}");
                this.IsValid = false;
            }
        }

        public bool IsValid { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();
    }
}

