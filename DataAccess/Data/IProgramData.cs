using SharedProject.Models;

namespace DataAccess.Data;
public interface IProgramData
{
    Task DeleteProgramAsync(int id);
    Task<IEnumerable<Program>> GetAllProgramsAsync();
    Task<Program> GetProgramAsync(int id);
    Task<int?> GetProgramIdAsync(string name);
    Task InsertProgramAsync(Program program);
    Task UpdateProgramAsync(Program program);
}