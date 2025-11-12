
namespace DataAccessLibrary.DataServices
{
    public interface IMainFormSampleCheckboxAdminService
    {
        Task CreateAndDeleteAsync(int mainFormId, List<int> checkboxIds);
        Task CreateAsync(int mainFormId, List<int> checkboxIds);
        Task DeleteByMainFormIdAsync(int mainFormId);
        Task<List<int>> GetSelectedSampleCheckboxAdminIdsByMainFormIdAsync(int mainFormId);
    }
}