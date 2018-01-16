using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bxl.Test {

    [TestFixture]
    public class SelectTabTesting {

        [Test]
        public void FigureOut() {


            //string filePath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
            //FileUpload1.SaveAs(filePath);
            var filePath = @"C:\Users\Nathan_Carroll\Documents\Projects\EOL\Dell_EMC_Service_EOL_Notification_Template_V1_test1.xlsx";
            //Open the Excel file in Read Mode using OpenXml.
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false)) {
                //Read the first Sheet from Excel file.
                //var sheets = doc.WorkbookPart.Workbook.Sheets.fi.ToList();
                //foreach (Sheet item in sheets) {
                //    var id = item.Id.Value;
                //    //and start on row 4 for read
                //    //Supplier Notification data

                //}

                ////Get the Worksheet instance.
                //Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
            }
        }
    }
}
