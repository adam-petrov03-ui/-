public class ConsoleView
{
    public void ShowMenu() // вывод меню
    {
        Console.Clear();
        Console.WriteLine("__Меню__");
        Console.WriteLine("1. Добавить задачу");
        Console.WriteLine("2. Показать все задачи");
        Console.WriteLine("3. Показать выполненные");
        Console.WriteLine("4. Показать невыполненные");
        Console.WriteLine("5. Отметить задачу выполненной");
        Console.WriteLine("0. Выход");
        Console.Write("Выберите команду: ");
    }

    public void ShowTasks(IEnumerable<TaskItem> tasks, string header) // вывод списка задач
    {
        Console.WriteLine($"\n__{header}__");

        var taskList = tasks.ToList();

        if (taskList.Count == 0)
        {
            Console.WriteLine("Задачи отсутствуют");
            return;
        }

        Console.WriteLine("{0,-5} {1,-40} {2}", "ID", "Название", "Статус");
        Console.WriteLine(new string('-', 60));

        foreach (var task in taskList)
        {
            var status = task.Status switch
            {
                TaskStatus.ToDo => "Не выполнена",
                TaskStatus.InProgress => "В работе",
                TaskStatus.Done => "Выполнена",
                _ => task.Status.ToString()
            };

            Console.WriteLine("{0,-5} {1,-40} {2}", task.Id, task.Title, status);
        }
    }

    public void ShowMessage(string message) // вывод сообщений. Красный цвет для ошибок
    {
        var previousColor = Console.ForegroundColor;

        try
        {
            if (message.StartsWith("Ошибка:", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }

    public string? GetInput() // получение ввода от пользователя
    {
        return Console.ReadLine();
    }

    public string? PromptForString(string prompt) // выводит запрос и возвращает введенную строку(для названия задачи)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}