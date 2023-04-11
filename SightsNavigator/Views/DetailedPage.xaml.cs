using SightsNavigator.Models;

namespace SightsNavigator.Views;

public partial class DetailedPage : ContentPage
{
	private City.Sight _sight; 
	public DetailedPage(City.Sight sight)
	{
		_sight = sight;
		InitializeComponent();
	}
}