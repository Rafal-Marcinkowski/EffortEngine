using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class ProgramData(ISQLDataAccess dbAccess) : IProgramData
{
    public async Task<IEnumerable<Program>> GetAllProgramsAsync()
    {
        string sql = "SELECT * FROM Programs";

        return await dbAccess.LoadDataWithQueryAsync<Program>(sql);
    }

    public async Task<Program> GetProgramAsync(int id)
    {
        string sql = "SELECT * FROM Programs WHERE Id = @Id";
        var programs = await dbAccess.LoadDataWithQueryAsync<Program>(sql, new { Id = id });

        return programs.FirstOrDefault();
    }

    public async Task InsertProgramAsync(Program program)
    {
        string sql = @"
            INSERT INTO Programs (Name, TotalWorkTime)
            VALUES (@Name, @TotalWorkTime)";
        await dbAccess.SaveDataWithQueryAsync(sql, program);
    }

    public async Task UpdateProgramAsync(Program program)
    {
        string sql = @"
            UPDATE Programs 
            SET Name = @Name, 
                TotalWorkTime = @TotalWorkTime
            WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, program);
    }

    public async Task DeleteProgramAsync(int id)
    {
        string sql = "DELETE FROM Programs WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, new { Id = id });
    }

    public async Task<int?> GetProgramIdAsync(string name)
    {
        string sql = "SELECT Id FROM Programs WHERE Name = @Name";
        var result = await dbAccess.LoadDataWithQueryAsync<int>(sql, new { Name = name });

        return result.FirstOrDefault();
    }
}