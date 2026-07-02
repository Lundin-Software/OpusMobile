using System.Text.Json.Serialization;
using Microsoft.Maui.Graphics;

namespace Opus.Mobile.Models.Modules.Todos;

public class TodoModel : Shared.Todos.TodoItem
{
    [JsonIgnore]
    public bool IsUnscheduled => Unscheduled.GetValueOrDefault() > 0;

    [JsonIgnore]
    public bool HasComponentTree => !string.IsNullOrWhiteSpace(ComponentTree);

    [JsonIgnore]
    public bool HasDescription => !string.IsNullOrWhiteSpace(Desc);

    [JsonIgnore]
    public string DisplayTaskTypeName => string.IsNullOrWhiteSpace(TaskTypeName)
        ? "No task type"
        : TaskTypeName;

    [JsonIgnore]
    public string DisplayResponsibleRoleName => string.IsNullOrWhiteSpace(ResponsibleRoleName)
        ? "No role"
        : ResponsibleRoleName;

    [JsonIgnore]
    public string RemainingText
    {
        get
        {
            if (LeftTime is null)
                return string.Empty;

            var value = LeftTime.Value.ToString("0.##");

            return string.IsNullOrWhiteSpace(LeftTimeChar)
                ? value
                : $"{value} {LeftTimeChar}";
        }
    }

    [JsonIgnore]
    public TodoVisualState VisualState =>
        LeftTime.GetValueOrDefault() < 0
            ? TodoVisualState.Overdue
            : TodoVisualState.Ok;

    [JsonIgnore]
    public Color StatusColor =>
        VisualState == TodoVisualState.Overdue
            ? Colors.Red
            : Colors.Green;
}

public enum TodoVisualState
{
    Overdue,
    Ok
}
