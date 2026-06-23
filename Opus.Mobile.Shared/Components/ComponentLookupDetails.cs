namespace Opus.Mobile.Shared.Components;

public class ComponentLookupDetails
{
    public ComponentDetailsItem? ComponentDetails { get; set; }

    public List<ComponentClassFieldItem> ComponentClassFields { get; set; } = [];

    public List<ComponentTaskItem> ComponentTasks { get; set; } = [];
}
