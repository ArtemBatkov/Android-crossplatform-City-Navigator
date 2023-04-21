using SightsNavigator.Models;
using SightsNavigator.Services;
using SightsNavigator.Services.SightService;
using SightsNavigator.ViewModels;
using SightsNavigator.Views;

namespace SightsNavigator.Views;

public partial class MainPage : ContentPage
{
	 
    private SearchCitySightsViewModel _sightViewModel;

    private IServiceProvider serviceProvider;
	public MainPage(IServiceProvider serviceProvider)
	{
		InitializeComponent();
        this.serviceProvider = serviceProvider;
        _sightViewModel= new SearchCitySightsViewModel();
        _sightViewModel.navigation = Navigation;
        _sightViewModel.ServiceProvider = serviceProvider;


        BindingContext = _sightViewModel;
        //_sightViewModel.SightsCollectionView = myCV;
    }

    private void SightsScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        
        _sightViewModel.onSightsScrolled(sender, e);
    }

    
}

