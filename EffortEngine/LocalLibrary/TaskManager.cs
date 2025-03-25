using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

public class TaskManager(ITaskData taskData, IProgramData programData) : BindableBase
{
    public static TaskBase CurrentTask { get; set; }
    public static Program CurrentProgram { get; set; }

    public async Task EvaluateTask(string taskName)
    {
        var task = (await taskData.GetAllTasksAsync()).FirstOrDefault(q => q.Name == taskName);

        if (task is not null)
        {
            CurrentTask = task;
            CurrentProgram = null;
        }

        else
        {
            CurrentProgram = (await programData.GetAllProgramsAsync()).FirstOrDefault(q => q.Name == taskName);
            CurrentTask = null;
        }
    }
}
