namespace Ass5.Models;

public sealed class SupabaseOptions
{
    public string Url { get; init; } = "https://YOUR-PROJECT.supabase.co";
    public string AnonKey { get; init; } = "YOUR-ANON-KEY";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Url) &&
        !string.IsNullOrWhiteSpace(AnonKey) &&
        !Url.Contains("YOUR-PROJECT", StringComparison.OrdinalIgnoreCase) &&
        !AnonKey.Contains("YOUR-ANON-KEY", StringComparison.OrdinalIgnoreCase);

    public static SupabaseOptions Load() => new()
    {
        Url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "https://YOUR-PROJECT.supabase.co",
        AnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? "YOUR-ANON-KEY"
    };
}
