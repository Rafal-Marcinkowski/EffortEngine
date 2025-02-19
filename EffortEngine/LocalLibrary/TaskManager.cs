using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

public class TaskManager(ITaskData taskData, IProgramData programData) : BindableBase
{
    public TaskBase CurrentTask { get; set; }
    public Program CurrentProgram { get; set; }

    public async Task EvaluateTask(string taskName)
    {
        var task = (await taskData.GetAllTasksAsync()).Where(q => q.Name == taskName);

        if (task != null)
        {
            CurrentTask = task.FirstOrDefault();
            CurrentProgram = null;
        }

        else
        {
            CurrentProgram = (Program)(await programData.GetAllProgramsAsync()).Where(q => q.Name == taskName);
            CurrentTask = null;
        }
    }
}
