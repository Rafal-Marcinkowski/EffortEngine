using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class TaskData(ISQLDataAccess dbAccess) : ITaskData
{
    public async Task<IEnumerable<TaskBase>> GetAllTasksAsync()
    {
        string sql = "SELECT * FROM Tasks";

        return await dbAccess.LoadDataWithQueryAsync<TaskBase>(sql);
    }

    public async Task<TaskBase> GetTaskAsync(int id)
    {
        string sql = "SELECT * FROM Tasks WHERE Id = @Id";
        var tasks = await dbAccess.LoadDataWithQueryAsync<TaskBase>(sql, new { Id = id });

        return tasks.FirstOrDefault();
    }

    public async Task InsertTaskAsync(TaskBase task)
    {
        string sql = @"
            INSERT INTO Tasks 
            (Name, Description, Priority, CreatedAt, DueDate, CompletionDate, Status, WorkTime, Note, ProgramId)
            VALUES 
            (@Name, @Description, @Priority, @CreatedAt, @DueDate, @CompletionDate, @Status, @WorkTime, @Note, @ProgramId)";
        await dbAccess.SaveDataWithQueryAsync(sql, task);
    }

    public async Task UpdateTaskAsync(TaskBase task)
    {
        string sql = @"
            UPDATE Tasks 
            SET Name = @Name, 
                Description = @Description, 
                Priority = @Priority, 
                CreatedAt = @CreatedAt, 
                DueDate = @DueDate, 
                CompletionDate = @CompletionDate, 
                Status = @Status, 
                WorkTime = @WorkTime, 
                Note = @Note,
                ProgramId = @ProgramId
            WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        string sql = "DELETE FROM Tasks WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, new { Id = id });
    }
}
