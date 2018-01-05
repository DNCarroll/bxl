using System.IO;

namespace bxl {
    public static class Construct {

        public static Model.Template Template(string referenceFile,
            bool purgeWorkingOnEveryRun,
            string processName, string processGroup,
            bool headers = true,
            bool isActive = true,
            string connectionString = null, string loadTransformProcedure = null, string fileNamePattern = null, string onErrorEmail = null) {
            Model.Template template = new Model.Template {
                PurgeWorkingOnEveryRun = purgeWorkingOnEveryRun,
                ProcessName = processName,
                ProjectGroup = processGroup,
                ConnectionString = connectionString,
                IsActive = isActive,
                LoadTransformProcedure = loadTransformProcedure,
                FileNamePattern = fileNamePattern,
                OnErrorEmail = onErrorEmail
            };            
            return Template(template, referenceFile);
        }

        public static Model.Template Template(Model.Template template, string referenceFile) {
            var temp = referenceFile.ToLower();
            var fileInfo = new FileInfo(referenceFile);
            var extension = fileInfo.Extension.ToLower();
            template.FileNamePattern = $"*{extension}";
            var dataHandler = Data.DataFileHandler.GetHandler(referenceFile);            
            if (extension == ".csv") {
                template.Delimiter = ",";
            }
            else if (extension == ".txt") {
                template.Delimiter = template.Delimiter ?? "\t";
            }
            else if (extension!=".xls" && extension != ".xlsx") {
                template.Delimiter = template.Delimiter ?? "\t";
            }
            template.Fields = dataHandler.FieldDefinitions(referenceFile, template);
            return template;
        }

        public static bool Save(Model.Template template) {
            bool saved = false;
            try {
                template.Update();
                if(template.TemplateId > 0) {
                    foreach (var field in template.Fields) {
                        field.TemplateId = template.TemplateId;
                        field.Update();
                        if(field.TemplateFieldId == 0) {
                            return false;
                        }
                    }
                    saved = true;
                }
            }
            catch{
                saved = false;
            }
            return saved;
        }
    }
}
