
namespace DataAccessLibrary.DataServices
{
    public interface IMainFormIncidentBehaviourTypeService
    {
        Task CreateAndDeleteAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task CreateAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task DeleteByMainFormIdAsync(int mainFormId);
        Task<List<int>> GetSelectedIncidentBehaviourTypeIdsByMainFormIdAsync(int mainFormId);
    }
}