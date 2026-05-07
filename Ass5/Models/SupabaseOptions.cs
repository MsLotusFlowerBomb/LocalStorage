namespace Ass5.Models;

public sealed class SupabaseOptions
{
    public string Url { get; init; } = string.Empty;
    public string AnonKey { get; init; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Url) &&
        !string.IsNullOrWhiteSpace(AnonKey);

    public static SupabaseOptions Load() => new()
    {
        Url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? string.Empty,
        AnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? string.Empty
    };
}
