using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace bxl.Data {

    public class Xlsx : DataFileHandler {
        public override List<Model.Field> FieldDefinitions(string fileName, Model.Template template) {
            var fields = new List<Model.Field>();
            int row = 0;
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false)) {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                var cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats;
                foreach (WorksheetPart worksheetPart in workbookPart.WorksheetParts) {
                    OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                    var needHeader = true;
                    while (reader.Read()) {
                        if (reader.ElementType == typeof(Row)) {
                            reader.ReadFirstChild();
                            int columnIndex = 0;
                            do {
                                if (reader.ElementType == typeof(Cell)) {
                                    Cell c = (Cell)reader.LoadCurrentElement();
                                    var cellValue = GetCellValue(workbookPart, cellFormats, c);
                                    if (needHeader) {
                                        SetHeader(template.HasHeaders, fields, c, cellValue);
                                        if (template.HasHeaders) {
                                            continue;
                                        }
                                    }
                                    string excelColumnReference = Regex.Replace(c.CellReference, "\\d", "");
                                    var field = fields.FirstOrDefault(f => f.ExcelColumnReference == excelColumnReference);
                                    if (field != null && field.ColumnIndex != columnIndex) {
                                        fields[columnIndex].IsNullable = true;
                                        columnIndex = field.ColumnIndex;
                                    }
                                    SetFieldType(fields, cellValue, c);
                                    columnIndex++;
                                }
                            } while (reader.ReadNextSibling());
                            needHeader = false;
                            row++;
                        }
                    }
                }
            }
            return fields;
        }

        void SetHeader(bool withHeaders, List<Model.Field> fields, Cell c, string cellValue) {
            var field = new Model.Field {
                ColumnIndex = fields.Count,
                FieldName = withHeaders ? cellValue : $"field{fields.Count + 1}",
                ExcelColumnReference = Regex.Replace(c.CellReference, "\\d", "")
            };
            fields.Add(field);
        }

        void SetFieldType(List<Model.Field> fields, string value, Cell cell) {
            string excelColumnReference = Regex.Replace(cell.CellReference, "\\d", "");
            var field = fields.FirstOrDefault(f => f.ExcelColumnReference == excelColumnReference);
            if (field != null) {
                if (!string.IsNullOrEmpty(value) && value.Length > field.ColumnLength) {
                    field.ColumnLength = (int)((double)value.Length * 1.8);
                }
                if (field.FieldType != Constants.String) {
                    var type = GetFieldType(value);
                    if (field.FieldType == null) {
                        field.FieldType = type;
                    }
                    else if (type != field.FieldType) {
                        field.FieldType = Constants.String;
                    }
                }
            }
        }

        string GetCellValue(WorkbookPart workbookPart, CellFormats cellFormats, Cell c) {
            string cellValue = c.CellValue.InnerText; 
            if (c.DataType != null && c.DataType == CellValues.SharedString) {
                SharedStringItem ssi = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(c.CellValue.InnerText));

                cellValue = ssi.Text.Text;
            }
            else if (c.StyleIndex != null) {
                var styleIndex = (int)c.StyleIndex.Value;
                var cellFormat = (CellFormat)cellFormats.ElementAt(styleIndex);
                cellValue = GetFormattedValue(cellFormat.NumberFormatId, c);
            }
            return cellValue;
        }

        string GetFormattedValue(UInt32Value numericId, Cell cell) {
            var value = cell.CellValue.InnerText;
            if (numericId != null) {
                int i = (int)numericId.Value;
                switch (i) {
                    case 14:
                    case 22:
                    case 55:
                        double dateAsDouble;
                        if(double.TryParse(value, out dateAsDouble)) {
                            value = DateTime.FromOADate(dateAsDouble).ToString(); break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return value;
        }

        internal override void ImportSubhandler(ImportResults import) {

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(import.ImportFileName, false)) {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                var cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats;
                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                var needHeader = true;
                List<Model.Field> fields = new List<Model.Field>();
                int rowIndex = 1;
                while (reader.Read()) {
                    if (reader.ElementType == typeof(Row)) {
                        reader.ReadFirstChild();
                        fields.ForEach(f => f.Value = DBNull.Value);
                        ReadCells(import, workbookPart, cellFormats, reader, needHeader, fields, rowIndex);
                        if (needHeader) {
                            needHeader = false;
                            if (!import.Template.HasGoodColumnMatch(fields)) {
                                import.Message = $"File formatting issue: file,{import.ImportFileName}, columns do not match the reference file,{import.Template.ReferenceFile}";
                                this.ShouldAttemptInsert = false;
                                return;
                            }
                            if (import.Template.HasHeaders) {
                                continue;
                            }
                        }
                        this.Insert(fields, import, rowIndex);
                        rowIndex++;
                    }
                }
            }
        }

        private void ReadCells(ImportResults import, WorkbookPart workbookPart, CellFormats cellFormats,
                OpenXmlReader reader, bool needHeader, List<Model.Field> fields, int rowIndex) {
            do {
                if (reader.ElementType == typeof(Cell)) {
                    Cell c = (Cell)reader.LoadCurrentElement();
                    var cellValue = GetCellValue(workbookPart, cellFormats, c);
                    if (needHeader) {
                        SetHeader(import.Template.HasHeaders, fields, c, cellValue);
                        if (import.Template.HasHeaders) {
                            continue;
                        }
                    }
                    string excelColumnReference = Regex.Replace(c.CellReference, "\\d", "");
                    var field = fields.FirstOrDefault(f => f.ExcelColumnReference == excelColumnReference);
                    SetFieldType(fields, cellValue, c);
                    if (field != null) {
                        field.SetValue(cellValue);
                    }
                }
            } while (reader.ReadNextSibling());
        }
    }
}