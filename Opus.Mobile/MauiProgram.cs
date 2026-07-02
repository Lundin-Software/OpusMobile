using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Opus.Mobile.Models;
using Opus.Mobile.Services.Modules.Authentication;
using Opus.Mobile.Services.Modules.Todos;
using Opus.Mobile.Services.Requests.RequestProcessors;
using Opus.Mobile.Services.Requests.RequestProvider;
using Opus.Mobile.ViewModels;
using Opus.Mobile.ViewModels.Todos;
using Opus.Mobile.Views.Login;
using Opus.Mobile.Views.Todos;
using UraniumUI;
using ZXing.Net.Maui.Controls;

namespace Opus.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseBarcodeReader()    
                .ConfigureMopups()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);

                    fonts.AddFontAwesomeIconFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<ModalErrorHandler>();

            //Services
            builder.Services.AddSingleton<IRequestProvider, RequestProvider>();
            builder.Services.AddSingleton<IRequestProcessor, OpusRequestProcessor>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<ILoginPreferencesStore, LoginPreferencesStore>();
            builder.Services.AddSingleton<ITodosService, TodosService>();

            //Views
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<TodosPage>();

            //ViewModels
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<TodosViewModel>();

            //API URL
            ApplicationSettings.ApiBaseUrl = "http://192.168.69.185:5181/";

            return builder.Build();
        }
    }
}
