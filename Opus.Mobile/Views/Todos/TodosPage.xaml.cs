using Opus.Mobile.ViewModels.Todos;

namespace Opus.Mobile.Views.Todos;

public partial class TodosPage : ContentPage
{
    private readonly TodosViewModel viewModel;

    public TodosPage(TodosViewModel viewModel)
	{
		InitializeComponent();

        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (viewModel.Todos.Count == 0)
            viewModel.LoadTodosCommand.Execute(null);
    }
}