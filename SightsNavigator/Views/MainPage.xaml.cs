using SightsNavigator.Models;
using SightsNavigator.Services;
using SightsNavigator.Services.SightService;
using SightsNavigator.ViewModels;
using SightsNavigator.Views;

namespace SightsNavigator.Views;

public partial class MainPage : ContentPage
{
	 

	public MainPage()
	{
		InitializeComponent();

        BindingContext = new SearchCitySightsViewModel();


    }
    
    
      
}

