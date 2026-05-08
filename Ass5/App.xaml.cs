using System.Diagnostics;
using Ass5.Services;

namespace Ass5
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            Task.Run(async () =>
            {
                try
                {
                    var service = Current?.Handler?.MauiContext?.Services?.GetService<IShoppingDataService>();
                    if (service is null)
                    {
                        Debug.WriteLine("Supabase: IShoppingDataService not available from DI.");
                        return;
                    }

                    if (service is SupabaseShoppingDataService supabase)
                    {
                        var ok = await supabase.CheckConnectionAsync();
                        Debug.WriteLine(ok
                            ? "Supabase: connection OK"
                            : "Supabase: connection FAILED (check SUPABASE_URL / SUPABASE_ANON_KEY and RLS policies)");
                    }
                    else
                    {
                        Debug.WriteLine($"Supabase: Active data service is '{service.GetType().Name}', connection check skipped.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Supabase: connection check threw an exception: {ex.Message}");
                }
            });

            return window;
        }
    }
}
