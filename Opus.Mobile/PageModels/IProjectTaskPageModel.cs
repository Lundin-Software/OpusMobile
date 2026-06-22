using CommunityToolkit.Mvvm.Input;
using Opus.Mobile.Models;

namespace Opus.Mobile.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}