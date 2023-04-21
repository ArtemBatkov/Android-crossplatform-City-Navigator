using SightsNavigator.Models;
using SightsNavigator.ViewModels;
using System.ComponentModel.Design;

namespace SightsNavigator.Views;

public partial class TripDetailedPage : ContentPage
{


    private TripDetailedViewModel _tripDetailedViewModel;

    private IServiceProvider _serviceProvider;
    private City city;

    private TripEditViewModel _tripEditViewModel;
    public TripDetailedPage(City city, TripEditViewModel tripEditViewModel, IServiceProvider serviceProvider)
	{
		InitializeComponent();
        this._serviceProvider = serviceProvider;
        this._tripEditViewModel = tripEditViewModel;
        this.city = city;      
        this._tripDetailedViewModel = new TripDetailedViewModel(city, _tripEditViewModel, _serviceProvider);        
        _tripDetailedViewModel.navigation = Navigation;
        var uri = new Uri(city.CurrentBackground);
        var urimage = new UriImageSource();
        urimage.Uri = uri;
        _tripDetailedViewModel.BackgroundImage = urimage;
        //_tripDetailedViewModel.city = city;
        //_tripDetailedViewModel.BackgroundImage = city.CurrentBackground        
        BindingContext = _tripDetailedViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();       
        OnPropertyChanged(nameof(_tripDetailedViewModel.city));

        var actualFav = _tripDetailedViewModel.city.FavouriteSights;
        var factFav = _tripDetailedViewModel.Favourites;
        if(actualFav.Count != factFav.Count)
        {
            _tripDetailedViewModel.Favourites.Clear();
            actualFav.ForEach(d => _tripDetailedViewModel.Favourites.Insert(0, d));
        }

        OnPropertyChanged(nameof(_tripDetailedViewModel.Favourites));
        OnPropertyChanged(nameof(_tripDetailedViewModel.FavouriteSelected));

        var uri = new Uri(city.CurrentBackground);
        var urimage = new UriImageSource();
        urimage.Uri = uri;
        _tripDetailedViewModel.BackgroundImage = urimage;
        OnPropertyChanged(nameof(_tripDetailedViewModel.BackgroundImage));


    }
}