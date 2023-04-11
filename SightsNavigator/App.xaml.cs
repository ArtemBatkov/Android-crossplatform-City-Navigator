using SightsNavigator.Services.SightService;
using SightsNavigator.Services;
using SightsNavigator.Views;
using SightsNavigator.Models;
using SightsNavigator.Services.NavigationService;

namespace SightsNavigator;

public partial class App : Application
{
    private City.Sight _selectedItem = new City.Sight();
	public App()
	{
		InitializeComponent();
        DependencyService.Register<SightRequest>();     
        DependencyService.Register<WebRequest>();

        INavigation navigation = MainPage.Navigation;
        DependencyService.Register<INavigation>(navigation);
        DependencyService.Register<INavigationService>(() => new NavigationService(navigation));

        _selectedItem.Description = "DESCRIPTION";

        //MainPage = new AppShell();
        MainPage = new NavigationPage(new MainPage());
        //MainPage.Navigation.PushAsync(new DetailedPage(_selectedItem));
        //MainPage.Navigation.Add(new DetailedPage(_selectedItem));
    }
}
