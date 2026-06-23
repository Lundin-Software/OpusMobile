namespace Opus.Mobile.Shared.Components;

public class ComponentTreeItem
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public List<ComponentTreeItem> Children { get; set; } = [];
}
