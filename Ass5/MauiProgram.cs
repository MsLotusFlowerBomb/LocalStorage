using Ass5.Models;
using Ass5.Services;
using Ass5.ViewModels;
using Microsoft.Extensions.Logging;

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

        var supabaseOptions = SupabaseOptions.Load();

        builder.Services.AddSingleton(supabaseOptions);
        builder.Services.AddHttpClient<IShoppingDataService, SupabaseShoppingDataService>();

        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<ShoppingListViewModel>();
        builder.Services.AddSingleton<ShoppingCartViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
