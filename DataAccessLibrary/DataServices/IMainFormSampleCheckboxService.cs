
namespace DataAccessLibrary.DataServices
{
    public interface IMainFormSampleCheckboxService
    {
        Task CreateAndDeleteAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task CreateAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task DeleteByMainFormIdAsync(int mainFormId);
        Task<List<int>> GetSelectedSampleCheckboxIdsByMainFormIdAsync(int mainFormId);
    }
}