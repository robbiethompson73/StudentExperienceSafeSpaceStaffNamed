
namespace DataAccessLibrary.DataServices
{
    public interface IMainFormImpactedPersonTypeService
    {
        Task CreateAndDeleteAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task CreateAsync(int mainFormId, List<int> sampleCheckboxIds);
        Task DeleteByMainFormIdAsync(int mainFormId);
        Task<List<int>> GetSelectedImpactedPersonTypeIdsByMainFormIdAsync(int mainFormId);
    }
}