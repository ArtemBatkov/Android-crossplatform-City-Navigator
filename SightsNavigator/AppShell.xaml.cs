namespace SightsNavigator;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
	      Routing.RegisterRoute(nameof(Views.DetailedPage), typeof(Views.DetailedPage));
    }
}
