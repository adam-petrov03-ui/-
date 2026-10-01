public class TaskItem //класс сущности задачи
{
public int Id {get; set;} 
public string Title {get; set;} = string.Empty;
public TaskStatus Status {get; set;} 
}