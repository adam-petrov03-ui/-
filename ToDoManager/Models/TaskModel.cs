public class TaskModel 
{
    private readonly List<TaskItem> _tasks = new(); //список задач

    private int _freeId = 1; //свободный id для новой задачи

    public TaskItem AddTask(string title) // добавление задачи
    {
        var task = new TaskItem
        {
            Id = _freeId++,
            Title = title,
            Status = TaskStatus.ToDo
        };

        _tasks.Add(task);
        return task;
    }

    public IEnumerable<TaskItem> GetAllTasks() // получить все задачи
    {
        return _tasks;
    }

    public IEnumerable<TaskItem> GetTaskByStatus(TaskStatus status) // получить задачи по статусу
    {
        return _tasks.Where(t => t.Status == status);
    }
    public bool CompleteTaskById(int id) // найти задачу по id и изменить статус на Done
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Status = TaskStatus.Done;
            return true; //Задача найдена по id и статус изменен
        }
        return false; // Задача с таким id не найдена
    }

} 