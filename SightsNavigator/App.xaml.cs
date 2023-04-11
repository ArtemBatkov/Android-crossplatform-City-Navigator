using SightsNavigator.Services.SightService;
using SightsNavigator.Services;
using SightsNavigator.Views;
using SightsNavigator.Models;
using SightsNavigator.Services.NavigationService;

namespace SightsNavigator;

public partial class App : Application
{
   // private City.Sight _selectedItem = new City.Sight();
	public App()
	{
		InitializeComponent();
        DependencyService.Register<SightRequest>();     
        DependencyService.Register<WebRequest>();
       //  DependencyService.Register<NavigationService>();


        MainPage = new AppShell();
        //MainPage = new NavigationPage(new MainPage()); // задаем главную страницу как NavigationPage
        //MainPage.Navigation.PushAsync(new DetailedPage(_selectedItem));

    }
}
