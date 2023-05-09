
using CommunityToolkit.Maui;
using Firebase.Database;
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
        gkey = getAccess().Result;
        
        try { 
            if (!gkey[0].Equals('A'))
            {
                throw new Exception("Access Denied");
            }
        }catch(Exception ex)
        {
            throw new Exception("gkey is Empty");
        }

        builder.Services.AddSingleton<IGooglePlaceService>(sp => new GooglePlaceService(gkey));

        builder.UseMauiApp<App>()
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



    private static async Task<string> getAccess()
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://maui-citynavigator-default-rtdb.firebaseio.com");
        var gk = firebaseClient.Child("getaccess");
        var data = gk.OnceSingleAsync<string>();
        return data.Result;
    }



}


/*
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

               SecureStorage.Default.SetAsync("oauth_token", gkey);
           }
           catch
           {
               string oauthToken = SecureStorage.Default.GetAsync("oauth_token").Result;

               if (oauthToken == null)
               {
                   // No value is associated with the key "oauth_token"
                   throw new ArgumentNullException(nameof(oauthToken));
               }
               gkey = oauthToken;
           }
           finally
           {
               builder.Services.AddSingleton<IGooglePlaceService>(sp => new GooglePlaceService(gkey));
           }
       */