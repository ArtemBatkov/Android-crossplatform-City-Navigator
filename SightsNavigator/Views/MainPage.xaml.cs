using SightsNavigator.Models;
using SightsNavigator.Services;
using SightsNavigator.Services.SightService;
using SightsNavigator.Views;

namespace SightsNavigator.Views;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		
		
    }
    SightRequest service = new SightRequest();
    
    private async void OnCounterClicked(object sender, EventArgs e)
	{
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);

        City city = await service.GetCityAsync("Moscow");
        if (city != null)
        {
            LabelInfoCity.Text = $"name: {city.Name}\n" +
                $"country: {city.Country}\n" +
                $"lat: {city.Lat}\n" +
                $"lon: {city.Lon}\n" +
                $"population: {city.Country}\n" +
                $"timezone: {city.Timezone}\n" +
                $"status: {city.Status}";
        }
        else LabelInfoCity.Text = "NULL";
    }
}

