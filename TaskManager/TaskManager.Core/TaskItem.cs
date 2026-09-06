using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.Core;

public class TaskItem
{
    [BsonId]
    public string Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; } // a short description
    public TaskItemStatus Status { get; private set; } // if it is concluded or not

    public TaskItem(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
        Status = TaskItemStatus.Pending;
    }

    public bool ConcludeTask()
    {
        if (Status == TaskItemStatus.Pending)
        {
            Status = TaskItemStatus.Concluded;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        string checkMark = Status == TaskItemStatus.Concluded ? "[X]" : "[ ]";

        return $"{checkMark} {Name} - {Description} (ID: {Id})";
    }

}