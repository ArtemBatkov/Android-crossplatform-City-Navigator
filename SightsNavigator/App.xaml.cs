using SightsNavigator.Services.SightService;
using SightsNavigator.Services;

namespace SightsNavigator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        DependencyService.Register<SightRequest>();
     
        DependencyService.Register<WebRequest>();


        MainPage = new AppShell();
     
    }
}
