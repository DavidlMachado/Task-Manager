namespace TaskManager.Core;

public interface ITaskRepository 
{
    bool AddTask(TaskItem newTask);
    bool RemoveTask(string taskId);
    bool GetTask(string taskId, out TaskItem foundTask);
    IEnumerable<TaskItem> GetAllTasks();
    bool UpdateTask(TaskItem updatedTask);
}