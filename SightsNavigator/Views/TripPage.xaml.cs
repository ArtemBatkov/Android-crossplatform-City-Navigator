using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class TripPage : ContentPage
{
    private TripViewModel _tripViewModel;
    public TripPage()
    {
        InitializeComponent();
        _tripViewModel = new TripViewModel();
        BindingContext = _tripViewModel;
        _tripViewModel.navigation = Navigation;
    }
}