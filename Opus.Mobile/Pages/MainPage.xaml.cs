using Opus.Mobile.Models;
using Opus.Mobile.PageModels;

namespace Opus.Mobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}