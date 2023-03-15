using SightsNavigator.Services.SightService;
using SightsNavigator.Services;

namespace SightsNavigator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
        DependencyService.Register<ISightRequest>();
        DependencyService.Register<IWebRequest>();
    }
}
