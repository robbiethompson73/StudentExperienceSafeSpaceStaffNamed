
namespace DataAccessLibrary.DataAccessObjects
{
    public interface IDataAccess
    {
        Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName);
        Task<int> SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName);
    }
}