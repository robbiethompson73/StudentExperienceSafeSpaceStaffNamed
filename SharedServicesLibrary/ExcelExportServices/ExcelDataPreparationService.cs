using ClosedXML.Excel;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.ExcelExportServices
{
    public class ExcelDataPreparationService : IExcelDataPreparationService
    {

        // Define your data model here or pass it as parameter
        public byte[] BuildExcelExportFile(IEnumerable<ExcelEntityModel> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Export Data");

            // Headers
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "IncidentDate";
            worksheet.Cell(1, 3).Value = "StaffFullName";
            worksheet.Cell(1, 4).Value = "IncidentDetails";
            worksheet.Cell(1, 5).Value = "DateSubmitted";




            // Styling header
            worksheet.Range("A1:E1").Style.Font.Bold = true;
            worksheet.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.IncidentDate;
                worksheet.Cell(row, 3).Value = item.StaffFullName;
                worksheet.Cell(row, 4).Value = item.IncidentDetails;
                worksheet.Cell(row, 5).Value = item.DateSubmitted;

                row++;
            }

            // Borders around all used cells
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Auto fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }



    }
}
