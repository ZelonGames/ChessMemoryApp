namespace ChessMemoryApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(CustomVariationSelectorPage), typeof(CustomVariationSelectorPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(CustomVariationPage), typeof(CustomVariationPage));
    }
}
