using SharedProject.Models;

namespace DataAccess.Data;
public interface ITaskData
{
    Task DeleteTaskAsync(int id);
    Task<IEnumerable<TaskBase>> GetAllTasksAsync();
    Task<TaskBase> GetTaskAsync(int id);
    Task<int?> GetTaskIdAsync(string name);
    Task InsertTaskAsync(TaskBase task);
    Task UpdateTaskAsync(TaskBase task);
}