namespace TaskManager.Core;

public class InMemoryTaskRepository : ITaskRepository 
{
    private Dictionary<string, TaskItem> _tasks = new();

    public bool AddTask(TaskItem newTask) => _tasks.TryAdd(newTask.Id, newTask);
    public bool RemoveTask(string taskId) => _tasks.Remove(taskId);
    public bool GetTask(string taskId, out TaskItem foundTask) => _tasks.TryGetValue(taskId, out foundTask);
    public IEnumerable<TaskItem> GetAllTasks() => _tasks.Values;
    public bool UpdateTask(TaskItem updatedTask)
    {
        if (!_tasks.ContainsKey(updatedTask.Id))
        {
            return false;
        }

        _tasks[updatedTask.Id] = updatedTask;
        return true;
    }
}