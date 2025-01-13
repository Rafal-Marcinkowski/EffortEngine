
namespace DataAccess.DBAccess;

public interface ISQLDataAccess
{
    Task<IEnumerable<T>> LoadDataWithQueryAsync<T>(string sqlQuery, object parameters = null);
    Task SaveDataWithQueryAsync(string sqlQuery, object parameters = null);
}