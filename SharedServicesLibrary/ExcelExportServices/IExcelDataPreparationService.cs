using DataAccessLibrary.Models;

namespace SharedServicesLibrary.ExcelExportServices
{
    public interface IExcelDataPreparationService
    {
        byte[] BuildExcelExportFile(IEnumerable<ExcelEntityModel> data);
    }
}