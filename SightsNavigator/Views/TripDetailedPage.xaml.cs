using SightsNavigator.Models;
using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class TripDetailedPage : ContentPage
{

    private TripDetailedViewModel _tripDetailedViewModel;

    private IServiceProvider serviceProvider;
    private City city;

    public TripDetailedPage(City city)
	{
		InitializeComponent();
        this.city = city;      
        this._tripDetailedViewModel = new TripDetailedViewModel();
        BindingContext = _tripDetailedViewModel;
        _tripDetailedViewModel.navigation = Navigation;
        _tripDetailedViewModel.city = city;
        _tripDetailedViewModel.BackgroundImage = city.CurrentBackground;

       
    }
}