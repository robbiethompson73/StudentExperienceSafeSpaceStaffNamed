
namespace DataAccessLibrary.DataServices
{
    public interface IMainFormIncidentMotivationTypeService
    {
        Task CreateAndDeleteAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task CreateAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task DeleteByMainFormIdAsync(int mainFormId);
        Task<List<int>> GetSelectedIncidentMotivationTypeIdsByMainFormIdAsync(int mainFormId);
    }
}