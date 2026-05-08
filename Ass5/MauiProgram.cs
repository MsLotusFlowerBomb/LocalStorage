using Ass5.Models;
using Ass5.Services;
using Ass5.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // Add this using directive
using System.Net.Http; // Add this using directive
using Supabase; // Add this using directive for Supabase

namespace Ass5;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        //Supabase Setup
        var url = "https://nnrdetltgqwwzrougctr.supabase.co";
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5ucmRldGx0Z3F3d3pyb3VnY3RyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzgxNjU2NDMsImV4cCI6MjA5Mzc0MTY0M30.DqXq8Dn_ohwRkcUaVRsl8J2EAOibYE9xcHkO83RBkps";
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
            };

        //@TODO: review if we need to set up a custom HttpClient or if the default one is sufficient for our needs
        var supabaseOptions = Models.SupabaseOptions.Load();


        builder.Services.AddSingleton(supabaseOptions);
        builder.Services.AddHttpClient<IShoppingDataService, SupabaseShoppingDataService>();

        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<ShoppingListViewModel>();
        builder.Services.AddSingleton<ShoppingCartViewModel>();

        // Register the Supabase client as a singleton service
        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
