using TaskManager.Core;

namespace TaskManager.Tests;

public class TaskItemTests
{
    [Fact]
    public void ConcludeTask_ShouldChangeStatusToConcluded()
    {
        TaskItem tarefa = new TaskItem("Aprender Testes", "Configurar o projeto");

        tarefa.ConcludeTask();

        Assert.Equal(TaskItemStatus.Concluded, tarefa.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TaskItem_Constructor_ShouldThrowException_WhenNameIsInvalid(string invalidName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new TaskItem(invalidName, "Uma descrição válida"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TaskItem_Constructor_ShouldThrowException_WhenDescriptionIsInvalid(string invalidDescription)
    {
        Assert.ThrowsAny<ArgumentException>(() => new TaskItem("Um nome válido", invalidDescription));
    }
}