using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;

namespace DataAccess.DBAccess;

public class SQLDataAccess(IConfiguration configuration, ILogger logger) : ISQLDataAccess
{
    public async Task<IEnumerable<T>> LoadDataWithQueryAsync<T>(string sqlQuery, object parameters = null)
    {
        using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("EffortEngine_DBConnectionString"));
        return await dbConnection.QueryAsync<T>(sqlQuery, parameters);
    }

    public async Task SaveDataWithQueryAsync(string sqlQuery, object parameters = null)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString("EffortEngine_DBConnectionString"));
            await dbConnection.ExecuteAsync(sqlQuery, parameters);
        }

        catch (Exception ex)
        {
            logger.Error($"Błąd przy zapisywaniu danych do bazy danych: {ex.Message}", ex);
        }
    }
}
