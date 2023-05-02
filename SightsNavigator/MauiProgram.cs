
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using SightsNavigator.Services.GooglePlaceService;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SightsNavigator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        //If you can't see your path of "secrets.json", 
        /*
		 How to specify the path:
		1) click on your file right click, "secrets.json"
		2) properties
		3) Copy output directories = Copy always
		Finally it will set-up secrets.json
		 */

        string gkey = "NULL";
        try
        {





            var n = "secrets.json";
            //var dir = @"D:\Android C# Template Projects\SightsNavigator\SightsNavigator\secrets.json";

            var сукаПуть = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), n);
            //Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), n);
            // Path.Combine(Directory.GetCurrentDirectory(), n);

            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("SightsNavigator.secrets.json");

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            //var configuration = JsonConvert.DeserializeObject<Configuration>(json);

            var tempPath = Path.Combine(Path.GetTempPath(), n);
            File.WriteAllText(tempPath, json);


            var AppSettings = new ConfigurationBuilder()
                .AddJsonFile(tempPath)
                .Build();



            gkey = AppSettings["GoogleMapsApiKey"];
        }
        catch
        {

            //Polygone
            Task.Run(async () =>
            {
                //await SecureStorage.Default.SetAsync("oauth_token", "26110");
                string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");

                if (oauthToken == null)
                {
                    // No value is associated with the key "oauth_token"
                    throw new ArgumentNullException(nameof(oauthToken));
                }

                gkey = oauthToken;
            }
            );
        }

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
