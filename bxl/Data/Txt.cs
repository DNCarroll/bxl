using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace bxl.Data {
    public class Txt : DataFileHandler {

        public Txt() { }

        public override List<Model.Field> FieldDefinitions(string referenceFile, Model.Template template) {
            var fields = new List<Model.Field>();
            using(var sr = new StreamReader(referenceFile)) {
                while (sr.Peek() > -1) {
                    var line = sr.ReadLine();
                    if (line != "") {
                        var row = getSplit(template.Delimiter, line);
                        if (fields.Count == 0) {
                            if (template.HasHeaders) {
                                CreateFieldsFromHeader(fields, row);
                                continue;
                            }
                            else {
                                CreateFieldsWithNoHeader(fields, row);
                            }
                        }
                        if (fields.Count != row.Count()) {
                            throw new Exception("Bad file format.  Column counts vary within the file");
                        }
                        SetFieldType(fields, row);
                    }
                }
            }
            return fields;
        }

        string[] getSplit(string delimiter, string line) {
            if (delimiter != ",") {
                return line.Split(new string[] { delimiter }, StringSplitOptions.None);
            }
            else {
                //return Regex.Split(line, @"(?:,\s+)|(['""].+['""])(?:,\s+)").Where(s2 => !string.IsNullOrEmpty(s2)).ToArray();

                //var split = new List<string>();                
                //Regex regex = new Regex(",\\s+(?=(?:(?:[^\"]*\"){2})*[^\"]*$)");
                //foreach (Match m in regex.Matches(line)) {
                //    split.Add(m.Value);
                //}
                //return split.ToArray();
                return splitQuoted(line, ',');
            }
        }

        private string[] splitQuoted(string line, char delimeter) {
            string[] array;
            List<string> list = new List<string>();
            do {
                if (line.StartsWith("\"")) {
                    line = line.Substring(1);
                    int idx = line.IndexOf("\"");
                    while (line.IndexOf("\"", idx) == line.IndexOf("\"\"", idx)) {
                        idx = line.IndexOf("\"\"", idx) + 2;
                    }
                    idx = line.IndexOf("\"", idx);
                    list.Add(line.Substring(0, idx));
                    line = line.Substring(idx + 2);
                }
                else {
                    list.Add(line.Substring(0, Math.Max(line.IndexOf(delimeter), 0)));
                    line = line.Substring(line.IndexOf(delimeter) + 1);
                }
            }
            while (line.IndexOf(delimeter) != -1);
            list.Add(line);
            array = new string[list.Count];
            list.CopyTo(array);
            return array;
        }

        void SetFieldType(List<Model.Field> fields, string[] row) {
            foreach (var field in fields) {
                var value = row[field.ColumnIndex];
                if (field.FieldType != Constants.String) {                    
                    if (string.IsNullOrEmpty(value)) {
                        field.IsNullable = true;
                    }
                    else {
                        if (value.Length > field.ColumnLength) {
                            field.ColumnLength = (int)((double)value.Length * 1.8);
                        }
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

        void CreateFieldsWithNoHeader(List<Model.Field> fields, string[] row) {
            for (int i = 0; i < row.Length; i++) {
                fields.Add(new Model.Field {
                    ColumnIndex = i,
                    FieldName = $"field{i}"
                });
            }
        }

        void CreateFieldsFromHeader(List<Model.Field> fields, string[] row) {
            for (int i = 0; i < row.Length; i++) {
                fields.Add(new Model.Field {
                    ColumnIndex = i,
                    FieldName = row[i]
                });
            }
        }

        internal override void ImportSubhandler(ImportResults import) {
            var fields = new List<Model.Field>();
            int rowIndex = 1;
            using (var sr = new StreamReader(import.ImportFileName)) {
                while (sr.Peek() > -1) {
                    var line = sr.ReadLine();
                    if (line != "") {
                        var row = getSplit(import.Template.Delimiter, line);
                        if (fields.Count == 0) {
                            if (import.Template.HasHeaders) {
                                CreateFieldsFromHeader(fields, row);
                                continue;
                            }
                            else {
                                CreateFieldsWithNoHeader(fields, row);
                            }
                            if (!import.Template.HasGoodColumnMatch(fields)) {
                                import.Message = $"File formatting issue: file,{import.ImportFileName}, columns do not match the reference file,{import.Template.ReferenceFile}";
                                this.ShouldAttemptInsert = false;
                                return;
                            }
                        }
                        fields.ForEach(f => f.Value = DBNull.Value);
                        SetFieldType(fields, row);
                        this.Insert(fields, import, rowIndex);
                        rowIndex++;
                    }
                }
            }
        }
    }
}
