using ChessMemoryApp.ViewModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace ChessMemoryApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddSingleton<CourseSelectorPage>();

        builder.Services.AddSingleton<CustomVariationSelectorPage>();
        builder.Services.AddSingleton<CustomVariationSelectorViewModel>();

        builder.Services.AddTransient<CustomVariationPage>();
        builder.Services.AddTransient<CustomVariationViewModel>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<SearchPage>();

        builder.Services.AddTransient<OpeningPracticeViewModel>();
        builder.Services.AddTransient<OpeningPracticePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
