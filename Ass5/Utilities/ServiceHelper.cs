namespace Ass5.Utilities;

public static class ServiceHelper
{
    public static T GetService<T>() where T : notnull
    {
        var service = IPlatformApplication.Current?.Services.GetService(typeof(T)) as T;
        return service ?? throw new InvalidOperationException($"Service not found: {typeof(T).Name}");
    }
}
