using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class TripPage : ContentPage
{
    private TripViewModel _tripViewModel;
    private TripEditViewModel _tripEditViewModel;

    private IServiceProvider _serviceProvider;
    public TripPage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        _tripViewModel = new TripViewModel(_serviceProvider);         
        _tripViewModel.navigation = Navigation;         
        //_tripViewModel.PropertyChanged 
        BindingContext = _tripViewModel;
        //BindingContext.
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

      
        OnPropertyChanged(nameof(_tripViewModel.TripSelected));
        
        
    }
   
}