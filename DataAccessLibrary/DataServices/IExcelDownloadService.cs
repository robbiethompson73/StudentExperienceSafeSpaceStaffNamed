using DataAccessLibrary.Models;

namespace DataAccessLibrary.DataServices
{
    public interface IExcelDownloadService
    {
        Task<List<ExcelEntityModel>> GetDataForExcelExport();
    }
}