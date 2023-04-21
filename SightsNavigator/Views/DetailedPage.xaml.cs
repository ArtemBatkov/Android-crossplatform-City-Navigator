using SightsNavigator.Models;
using SightsNavigator.Services.GooglePlaceService;
using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class DetailedPage : ContentPage
{
	private City _city;
	private City.Sight _sight;
	private DetailedPageSightViewModel _detailedVM;
	public DetailedPage(City city, City.Sight sight, IServiceProvider serviceProvider)//City.Sight sight
    {
        InitializeComponent();
        _city = city;
		_sight = sight;
        _detailedVM = new DetailedPageSightViewModel(_city, _sight);
		
        BindingContext = _detailedVM;
        _detailedVM.ServiceProvider = serviceProvider;
        _detailedVM.Googleservices = serviceProvider.GetService<IGooglePlaceService>();
    }

   
}