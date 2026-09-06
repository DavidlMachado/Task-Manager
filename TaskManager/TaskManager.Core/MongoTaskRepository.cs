using MongoDB.Driver;

namespace TaskManager.Core;

public class MongoTaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskItem> _tasksCollection;

    public MongoTaskRepository()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("TaskManagerDB");       
        _tasksCollection = database.GetCollection<TaskItem>("TasksCollection"); 
    }

    public bool AddTask(TaskItem newTask)
    {
        try
        {
            _tasksCollection.InsertOne(newTask);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false;
        }
    }

    public bool RemoveTask(string taskId) 
    {
        
        var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, taskId);
        var result = _tasksCollection.DeleteOne(filter);
        return result.DeletedCount > 0;
    }

    public bool GetTask(string taskId, out TaskItem foundTask)
    {
        var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, taskId);
        foundTask = _tasksCollection.Find(filter).FirstOrDefault();
        return foundTask != null;

    }

    public IEnumerable<TaskItem> GetAllTasks()
    {
        return _tasksCollection.Find(Builders<TaskItem>.Filter.Empty).ToList();
    }

    public bool UpdateTask(TaskItem updatedTask)
    {
        var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, updatedTask.Id);
        var result = _tasksCollection.ReplaceOne(filter, updatedTask);
        return result.ModifiedCount > 0;
    }
}