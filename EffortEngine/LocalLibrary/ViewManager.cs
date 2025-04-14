using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

public class ViewManager(ITaskData taskData, IProgramData programData) : BindableBase
{
    public async Task<IEnumerable<TaskBase>> GetAllTasksAsync() =>
        [.. await taskData.GetAllTasksAsync()];

    public async Task<IEnumerable<TaskBase>> GetProgrammingTasksAsync() =>
        [.. (await taskData.GetAllTasksAsync()).Where(t => t.ProgramId != null)];

    public async Task<IEnumerable<TaskBase>> GetTasksByTypeAsync(TaskBase.TaskType type) =>
        [.. (await taskData.GetAllTasksAsync()).Where(t => t.Type == type)];

    public async Task<List<Program>> GetProgramsWithTasksAsync()
    {
        var programs = await programData.GetAllProgramsAsync();
        var allTasks = await taskData.GetAllTasksAsync();

        foreach (var program in programs)
        {
            var programId = await programData.GetProgramIdAsync(program.Name);

            program.Bugs = [.. allTasks.Where(t => t.ProgramId == programId && t.Type == TaskBase.TaskType.Bug).OfType<Bug>()];
            program.Features = [.. allTasks.Where(t => t.ProgramId == programId && t.Type == TaskBase.TaskType.Feature).OfType<Feature>()];
            program.Modules = [.. allTasks.Where(t => t.ProgramId == programId && t.Type == TaskBase.TaskType.Module).OfType<Module>()];
        }

        return [.. programs];
    }

    public async Task UpdateTaskAsync(TaskBase task) =>
        await taskData.UpdateTaskAsync(task);

    public async Task<TaskBase?> GetTaskByIdAsync(int id) =>
        await taskData.GetTaskAsync(id);

    public async Task<Program?> GetProgramByIdAsync(int id) =>
        await programData.GetProgramAsync(id);
}
