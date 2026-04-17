using System.Text.Json;
using Ass5.Models;

namespace Ass5.Services;

public sealed class ProfileStorage
{
    private const string ProfileFileName = "profile.json";
    private const string PhotoFileName = "profile-photo";

    public static string StorageDirectory => FileSystem.AppDataDirectory;
    public static string ProfileFilePath => Path.Combine(StorageDirectory, ProfileFileName);

    public async Task<Profile> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(ProfileFilePath))
            return new Profile();

        var json = await File.ReadAllTextAsync(ProfileFilePath, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
            return new Profile();

        return JsonSerializer.Deserialize<Profile>(json) ?? new Profile();
    }

    public async Task SaveAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory(StorageDirectory);
        await File.WriteAllTextAsync(ProfileFilePath, json, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> SavePhotoAsync(FileResult photo, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(photo.FileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".jpg";

        var fileName = $"{PhotoFileName}{ext}";
        var destPath = Path.Combine(StorageDirectory, fileName);

        await using var src = await photo.OpenReadAsync().ConfigureAwait(false);
        await using var dest = File.Open(destPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await src.CopyToAsync(dest, cancellationToken).ConfigureAwait(false);

        return fileName;
    }

    public string? GetPhotoPath(string? photoFileName)
    {
        if (string.IsNullOrWhiteSpace(photoFileName))
            return null;

        var path = Path.Combine(StorageDirectory, photoFileName);
        return File.Exists(path) ? path : null;
    }
}
