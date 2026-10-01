public class TaskController
{
    private readonly TaskModel _model;

    public TaskController(TaskModel model)
    {
        _model = model;
    }

    public TaskItem? AddTask(string? title) 
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        return _model.AddTask(title.Trim());
    }

    public IEnumerable<TaskItem> GetAllTasks() 
    {
        return _model.GetAllTasks();
    }

    public IEnumerable<TaskItem> GetCompletedTasks()
    {
        return _model.GetTaskByStatus(TaskStatus.Done);
    }

    public IEnumerable<TaskItem> GetUncompletedTasks()
    {
        return _model.GetTaskByStatus(TaskStatus.ToDo)
            .Concat(_model.GetTaskByStatus(TaskStatus.InProgress));
    }

    public bool CompleteTaskById(int id)
    {
        return _model.CompleteTaskById(id);
    }
}