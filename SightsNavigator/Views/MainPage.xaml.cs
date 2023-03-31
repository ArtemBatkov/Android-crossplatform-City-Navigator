using SightsNavigator.Models;
using SightsNavigator.Services;
using SightsNavigator.Services.SightService;
using SightsNavigator.ViewModels;
using SightsNavigator.Views;

namespace SightsNavigator.Views;

public partial class MainPage : ContentPage
{
	 
    private SearchCitySightsViewModel _sightViewModel;
	public MainPage()
	{
		InitializeComponent();
        _sightViewModel= new SearchCitySightsViewModel();

        BindingContext = _sightViewModel;
        _sightViewModel.SightsCollectionView = myCV;
    }

    private void SightsScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        _sightViewModel.onSightsScrolled(sender, e);
    }

    private void RemainingCollectionReached()
    {

    }
}

