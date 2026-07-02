using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Opus.Mobile.Models.Modules.Todos;
using Opus.Mobile.Services.Modules.Todos;
using Opus.Mobile.Services.NavigationService;
using Opus.Mobile.Shared.Todos;
using System.Collections.ObjectModel;

namespace Opus.Mobile.ViewModels.Todos;

public partial class TodosViewModel : BaseViewModel
{
    private readonly ITodosService todosService;
    private readonly List<TodoModel> scheduledTodos = [];
    private readonly List<TodoModel> unscheduledTodos = [];

    public ObservableCollection<TodoModel> Todos { get; } = [];

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUnscheduledSelected))]
    [NotifyPropertyChangedFor(nameof(EmptyText))]
    private bool isScheduledSelected = true;

    public bool IsUnscheduledSelected => !IsScheduledSelected;

    public int ScheduledCount => scheduledTodos.Count;

    public int UnscheduledCount => unscheduledTodos.Count;

    public string EmptyText => IsScheduledSelected
        ? "No scheduled tasks"
        : "No unscheduled tasks";

    public TodosViewModel(ITodosService todosService)
    {
        this.todosService = todosService;

        Title = "To Do";
    }

    [RelayCommand]
    private async Task LoadTodos()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            var response = await todosService.GetTodos(new TodoSearchRequest());

            if (response.Failed)
            {
                await ShowError(response.Message);
                return;
            }

            var todos = (response.Data ?? [])
                .Select(MapTodo)
                .ToList();

            scheduledTodos.Clear();
            unscheduledTodos.Clear();

            scheduledTodos.AddRange(todos.Where(todo => !todo.IsUnscheduled));
            unscheduledTodos.AddRange(todos.Where(todo => todo.IsUnscheduled));

            RefreshVisibleTodos();
        }
        catch (Exception ex)
        {
            await CaptureAndShowException("Something went wrong while loading todos.", ex);
        }
        finally
        {
            IsRefreshing = false;
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowScheduled()
    {
        if (IsScheduledSelected)
            return;

        IsScheduledSelected = true;
        RefreshVisibleTodos();
    }

    [RelayCommand]
    private void ShowUnscheduled()
    {
        if (IsUnscheduledSelected)
            return;

        IsScheduledSelected = false;
        RefreshVisibleTodos();
    }

    [RelayCommand]
    private async Task OpenTodo(TodoModel todo)
    {
        if (todo is null || IsBusy)
            return;

        await PopupNavigationService.ShowAlert("Opening the task is the next step.");
    }

    private void RefreshVisibleTodos()
    {
        Todos.Clear();

        var source = IsScheduledSelected
            ? scheduledTodos
            : unscheduledTodos;

        foreach (var todo in source)
            Todos.Add(todo);

        OnPropertyChanged(nameof(ScheduledCount));
        OnPropertyChanged(nameof(UnscheduledCount));
        OnPropertyChanged(nameof(EmptyText));
    }

    private static TodoModel MapTodo(TodoItem todo)
    {
        return new TodoModel
        {
            TodoId = todo.TodoId,
            TaskLogId = todo.TaskLogId,
            TaskId = todo.TaskId,
            TaskIntervalId = todo.TaskIntervalId,
            TaskClassId = todo.TaskClassId,

            Title = todo.Title,
            Desc = todo.Desc,

            LeftTime = todo.LeftTime,
            LeftTimeChar = todo.LeftTimeChar,
            IntervalChar = todo.IntervalChar,
            CompletedTime = todo.CompletedTime,

            ComponentId = todo.ComponentId,
            ComponentTree = todo.ComponentTree,
            ComponentHours = todo.ComponentHours,

            TaskTypeId = todo.TaskTypeId,
            TaskTypeName = todo.TaskTypeName,

            UserId = todo.UserId,
            UserShortName = todo.UserShortName,

            Unscheduled = todo.Unscheduled,
            ResponsibleRoleName = todo.ResponsibleRoleName
        };
    }

}