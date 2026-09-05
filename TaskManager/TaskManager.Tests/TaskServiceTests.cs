using TaskManager.Core;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    [Fact]
    public void AddTask_TaskIsNull_ThrowsArgumentNullException()
    {
        TaskItem newTask = null;

        TaskService taskService = new TaskService();

        Assert.Throws<ArgumentNullException>(() => taskService.AddTask(newTask));
    }

    [Fact]
    public void Remove_TaskIdIsNull_ThrowsArgumentNullException()
    {
        TaskService taskService = new TaskService();

        Assert.ThrowsAny<ArgumentException>(() => taskService.RemoveTask(null));
    }

    [Fact]
    public void GetTask_TaskIdIsNull_ThrowsArgumentNullException()
    {
        TaskService taskService = new TaskService();

        Assert.ThrowsAny<ArgumentException>(() => taskService.GetTask(null, out _));
    }

    [Fact]
    public void AddTask_ValidTask_ReturnsTrueAndAddsToList()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Garantir que a tarefa é adicionada");

        bool wasAdded = taskService.AddTask(newTask);

        Assert.True(wasAdded);

        Assert.Single(taskService.TaskList);

        Assert.True(taskService.TaskList.ContainsKey(newTask.Id));
    }

    [Fact]
    public void AddTask_DuplicateTask_ReturnsFalse()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Garantir que não adiciona duplicados");

        bool wasAdded = taskService.AddTask(newTask);

        Assert.True(wasAdded);

        bool addedDuplicate = taskService.AddTask(newTask);

        Assert.False(addedDuplicate);

        Assert.Single(taskService.TaskList);
    }

    [Fact]
    public void RemoveTask_ExistingId_ReturnsTrueAndRemovesTask()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Remover com id existente retorna true e remove");

        taskService.AddTask(newTask);

        bool wasRemoved = taskService.RemoveTask(newTask.Id);

        Assert.True(wasRemoved);

        Assert.Empty(taskService.TaskList);
    }

    [Fact]
    public void RemoveTask_NonExistingId_ReturnsFalse()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Remover com id inexistente retorna false e não remove");

        taskService.AddTask(newTask);

        bool wasRemoved = taskService.RemoveTask("id inexistente");

        Assert.False(wasRemoved);

        Assert.Single(taskService.TaskList);
    }

    [Fact]
    public void GetTask_ExistingId_ReturnsTrueAndOutputsTask()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Garantir que a tarefa é encontrada");

        taskService.AddTask(newTask);

        bool wasFound = taskService.GetTask(newTask.Id, out TaskItem foundTask);

        Assert.True(wasFound);
        Assert.NotNull(foundTask);
        Assert.Equal(newTask.Id, foundTask.Id);
    }

    [Fact]
    public void GetTask_NonExistingId_ReturnsFalse()
    {
        TaskService taskService = new TaskService();
        TaskItem newTask = new TaskItem("Testar", "Garantir que a tarefa não é encontrada");

        taskService.AddTask(newTask);

        bool wasFound = taskService.GetTask("id inexistente", out TaskItem foundTask);

        Assert.False(wasFound);
        Assert.Null(foundTask);
    }
    

}