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
            worksheet.Cell(1, 2).Value = "Date Submitted";
            worksheet.Cell(1, 3).Value = "Student Reference Number";
            worksheet.Cell(1, 4).Value = "Student DOB";
            worksheet.Cell(1, 5).Value = "Submitted By Windows Name";
            worksheet.Cell(1, 6).Value = "Full Name";
            worksheet.Cell(1, 7).Value = "SampleDropdown";
            worksheet.Cell(1, 8).Value = "SampleCheckbox Titles";



            // Styling header
            worksheet.Range("A1:H1").Style.Font.Bold = true;
            worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.DateSubmitted;
                worksheet.Cell(row, 3).Value = item.StudentReferenceNumber;
                worksheet.Cell(row, 4).Value = item.StudentDateOfBirth;
                worksheet.Cell(row, 5).Value = item.SubmittedByWindowsUserName;
                worksheet.Cell(row, 6).Value = item.StudentFullName;
                worksheet.Cell(row, 7).Value = item.SampleDropdownTitle;
                worksheet.Cell(row, 8).Value = item.SampleCheckboxTitles;

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
