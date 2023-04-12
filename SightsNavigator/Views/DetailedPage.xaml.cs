using SightsNavigator.Models;
using SightsNavigator.ViewModels;

namespace SightsNavigator.Views;

public partial class DetailedPage : ContentPage
{
	private City.Sight _sight;
	private DetailedPageSightViewModel _detailedVM;
	public DetailedPage(City.Sight sight)//City.Sight sight
    {
		_sight = sight;
        _detailedVM = new DetailedPageSightViewModel(_sight);
		 
		BindingContext = _detailedVM;
		

		InitializeComponent();
	}
}