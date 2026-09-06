namespace TaskManager.Core;

public class TaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public bool AddTask(TaskItem newTask) 
    {
        //if the task is null the system throws an exception
        ArgumentNullException.ThrowIfNull(newTask);

        return _repository.AddTask(newTask);
    }

    public bool RemoveTask(string taskId) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskId);

        return _repository.RemoveTask(taskId);
    }

    public bool GetTask(string taskId, out TaskItem foundTask) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskId);

        return _repository.GetTask(taskId,out foundTask);
    }

    public IEnumerable<TaskItem> GetAllTasks() => _repository.GetAllTasks();

    public bool UpdateTask(TaskItem updatedTask)
    {
        ArgumentNullException.ThrowIfNull(updatedTask);

        return _repository.UpdateTask(updatedTask);
    }
}