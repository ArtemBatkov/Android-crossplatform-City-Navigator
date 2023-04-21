using SightsNavigator.Models;
using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class TripEditPage : ContentPage
{
	private City city;
	private TripEditViewModel _tripEditViewModel;


	public TripEditPage(City city, TripEditViewModel tripEditViewModel)
	{
		InitializeComponent();
		this._tripEditViewModel = tripEditViewModel;
		this.city = city;
		//_tripEditViewModel = new TripEditViewModel(city);
		_tripEditViewModel.navigation = Navigation;
		BindingContext = _tripEditViewModel;
	}

    

    //  protected override bool OnBackButtonPressed()
    //  {
    //_tripEditViewModel.onBackPressedVM();
    //      return base.OnBackButtonPressed();
    //  }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _tripEditViewModel.Dispose();
    }

}