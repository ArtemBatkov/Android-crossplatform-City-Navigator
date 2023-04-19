using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SightsNavigator.Services.GooglePlaceService;

namespace SightsNavigator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

        var dir = @"D:\Android C# Template Projects\SightsNavigator\SightsNavigator\secrets.json";

        var AppSettings = new ConfigurationBuilder()
            .AddJsonFile(dir)
            .Build();

        var gkey = AppSettings["GoogleMapsApiKey"];

        builder.Services.AddSingleton<IGooglePlaceService>(sp => new GooglePlaceService(gkey));

        builder            
            .UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiMaps()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        
	 
		 

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
