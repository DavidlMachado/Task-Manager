namespace TaskManager.Core;

public class TaskService
{
    private Dictionary<string, TaskItem> _tasks = new Dictionary<string, TaskItem>();

    public IReadOnlyDictionary<string, TaskItem> TaskList => _tasks;

    public bool AddTask(TaskItem newTask)
    {
        //if the task is null the system throws an exception
        ArgumentNullException.ThrowIfNull(newTask);

        return _tasks.TryAdd(newTask.Id, newTask);
    }

    public bool RemoveTask(string taskId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskId);

        //the dictionary tries to remove the task
        return _tasks.Remove(taskId);
    }

    public bool GetTask(string taskId, out TaskItem foundTask)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskId);

        return TaskList.TryGetValue(taskId, out foundTask);
    }
}