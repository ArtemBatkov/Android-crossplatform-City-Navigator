using SightsNavigator.Services.SightService;
using SightsNavigator.Services;
using SightsNavigator.Views;
using SightsNavigator.Models;
using SightsNavigator.Services.NavigationService;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using SightsNavigator.Services.GooglePlaceService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;

using System.Windows;
using Autofac;

namespace SightsNavigator;

public partial class App : Application
{
    private IConfiguration Configuration { get; }


    private City.Sight _selectedItem = new City.Sight();
    public IServiceProvider ServiceProvider { get; }
    public App(IServiceProvider serviceProvider)
    {

        InitializeComponent();
        this.ServiceProvider = serviceProvider;
        //var AppSettings = new ConfigurationBuilder()
        //.AddJsonFile("secrets.json")
        //.Build();


        //var dir = @"D:\Android C# Template Projects\SightsNavigator\SightsNavigator\secrets.json";

        //var AppSettings = new ConfigurationBuilder()
        //    .AddJsonFile(dir)
        //    .Build();

        //var gkey = AppSettings["GoogleMapsApiKey"];

        //DependencyService.Register<GooglePlaceService>((service, arg) =>
        //{
        //    return new GooglePlaceService(arg);
        //}, gkey);

        //DependencyService.Register<IGooglePlaceService>(() => new GooglePlaceService(gkey));



        //DependencyService.Register<IGooglePlaceService>(serviceProvider =>
        //{
        //    return new GooglePlaceService(gkey);
        //});



        DependencyService.Register<SightRequest>();
        DependencyService.Register<WebRequest>();
        //DependencyService.Register<GooglePlaceService>();

        ;



        //var services = new ServiceCollection();
        //services.AddScoped<IGooglePlaceService>(provider =>
        //{
        //    var gkey = Configuration["GoogleMapsApiKey"];
        //    return new GooglePlaceService(gkey);
        //});

        //ServiceProvider = services.BuildServiceProvider();

        //INavigation navigation = MainPage.Navigation;
        //DependencyService.Register<NavigationService>(nabig);
        //DependencyService.Register<INavigation>(navigation);
        //DependencyService.Register<INavigationService>(() => new NavigationService(navigation));
        //DependencyService.Register<INavigationService>(() => new NavigationService(App.Current.MainPage.Navigation));



        _selectedItem.Description = "DESCRIPTION";

        //MainPage = new AppShell();





        MainPage = new NavigationPage(new MainPage(serviceProvider));
        //MainPage.Navigation.PushAsync(new DetailedPage(_selectedItem));
        //MainPage.Navigation.Add(new DetailedPage(_selectedItem));
        //DependencyService.Register<INavigationService>(() => new NavigationService(MainPage.Navigation));

        //DependencyService.Register<INavigationService, NavigationService>();
        Application.Current.UserAppTheme = AppTheme.Light;



    }




}
