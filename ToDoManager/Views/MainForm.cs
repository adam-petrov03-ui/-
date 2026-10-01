using System.Drawing;
using System.Windows.Forms;

public class MainForm : Form // класс формы для управления задачами
{
    private readonly TaskController _controller;
    private readonly TextBox _titleInput = new();
    private readonly ComboBox _filter = new();
    private readonly DataGridView _tasksGrid = new();
    private readonly Button _completeButton = new();
    private readonly Label _messageLabel = new();

    public MainForm()  // конструктор формы
    {
        _controller = new TaskController(new TaskModel());
        Text = "Менеджер задач";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 480);
        Size = new Size(900, 600);
        Font = new Font("Segoe UI", 10);

        BuildLayout();
        RefreshTasks();
    }

    private void BuildLayout() // метод для построения интерфейса формы
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(20)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        var heading = new Label
        {
            Text = "Задачи",
            Dock = DockStyle.Fill,
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(heading, 0, 0);

        var controls = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 5, 0, 5)
        };

        _titleInput.Width = 300;
        _titleInput.PlaceholderText = "Название задачи";
        _titleInput.Margin = new Padding(0, 3, 8, 0);
        _titleInput.KeyDown += (_, eventArgs) =>
        {
            if (eventArgs.KeyCode == Keys.Enter)
            {
                AddTask();
                eventArgs.SuppressKeyPress = true;
            }
        };

        var addButton = new Button
        {
            Text = "Добавить",
            AutoSize = true,
            Margin = new Padding(0, 0, 16, 0)
        };
        addButton.Click += (_, _) => AddTask();

        _filter.DropDownStyle = ComboBoxStyle.DropDownList;
        _filter.Width = 190;
        _filter.Items.AddRange(new object[] { "Все задачи", "Невыполненные", "Выполненные" });
        _filter.SelectedIndex = 0;
        _filter.Margin = new Padding(0, 3, 8, 0);
        _filter.SelectedIndexChanged += (_, _) => RefreshTasks();

        _completeButton.Text = "Отметить выполненной";
        _completeButton.AutoSize = true;
        _completeButton.Enabled = false;
        _completeButton.Click += (_, _) => CompleteSelectedTask();

        controls.Controls.Add(_titleInput);
        controls.Controls.Add(addButton);
        controls.Controls.Add(_filter);
        controls.Controls.Add(_completeButton);
        layout.Controls.Add(controls, 0, 1);

        _tasksGrid.Dock = DockStyle.Fill;
        _tasksGrid.ReadOnly = true;
        _tasksGrid.AllowUserToAddRows = false;
        _tasksGrid.AllowUserToDeleteRows = false;
        _tasksGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _tasksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _tasksGrid.MultiSelect = false;
        _tasksGrid.RowHeadersVisible = false;
        _tasksGrid.Columns.Add("Id", "ID");
        _tasksGrid.Columns.Add("Title", "Название");
        _tasksGrid.Columns.Add("Status", "Статус");
        _tasksGrid.Columns[0].FillWeight = 15;
        _tasksGrid.Columns[1].FillWeight = 60;
        _tasksGrid.Columns[2].FillWeight = 25;
        _tasksGrid.SelectionChanged += (_, _) => UpdateCompleteButton();
        layout.Controls.Add(_tasksGrid, 0, 2);

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(_messageLabel, 0, 3);

        Controls.Add(layout);
    }

    private void AddTask() // метод для добавления новой задачи
    {
        var task = _controller.AddTask(_titleInput.Text);
        if (task is null)
        {
            _messageLabel.Text = "Введите название задачи.";
            _titleInput.Focus();
            return;
        }

        _titleInput.Clear();
        _messageLabel.Text = $"Задача добавлена. ID: {task.Id}";
        RefreshTasks();
        _titleInput.Focus();
    }

    private void CompleteSelectedTask() // метод для отметки выбранной задачи как выполненной
    {
        if (_tasksGrid.SelectedRows.Count == 0 ||
            _tasksGrid.SelectedRows[0].Tag is not TaskItem task)
        {
            return;
        }

        if (_controller.CompleteTaskById(task.Id))
        {
            _messageLabel.Text = "Задача отмечена выполненной.";
            RefreshTasks();
        }
    }

    private void RefreshTasks() // обновляет список задач в таблице
    {
        var tasks = _filter.SelectedIndex switch
        {
            1 => _controller.GetUncompletedTasks(),
            2 => _controller.GetCompletedTasks(),
            _ => _controller.GetAllTasks()
        };

        _tasksGrid.Rows.Clear();
        foreach (var task in tasks)
        {
            var status = task.Status switch
            {
                TaskStatus.ToDo => "Не выполнена",
                TaskStatus.InProgress => "В работе",
                TaskStatus.Done => "Выполнена",
                _ => task.Status.ToString()
            };

            var rowIndex = _tasksGrid.Rows.Add(task.Id, task.Title, status);
            _tasksGrid.Rows[rowIndex].Tag = task;
        }

        UpdateCompleteButton();
    }

    private void UpdateCompleteButton() // обновляет состояния кнопки "Отметить выполненной"
    {
        _completeButton.Enabled = _tasksGrid.SelectedRows.Count > 0 &&
            _tasksGrid.SelectedRows[0].Tag is TaskItem task &&
            task.Status != TaskStatus.Done;
    }
}