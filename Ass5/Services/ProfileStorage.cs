using System.Text.Json;
using Ass5.Models;

namespace Ass5.Services;

// Service for loading and saving the profile data, including the profile photo. 
public sealed class ProfileStorage
{
    private const string ProfileFileName = "profile.json";
    private const string PhotoFileName = "profile-photo";

    public static string StorageDirectory => FileSystem.AppDataDirectory;
    public static string ProfileFilePath => Path.Combine(StorageDirectory, ProfileFileName);

    //Loads the profile data from a JSON file. If the file does not exist or is empty, it returns a new Profile.
    public async Task<Profile> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(ProfileFilePath))
            return new Profile();

        var json = await File.ReadAllTextAsync(ProfileFilePath, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
            return new Profile();

        return JsonSerializer.Deserialize<Profile>(json) ?? new Profile();
    }

    // Saves the profile data to a JSON file. It creates the storage directory if it does not exist.
    public async Task SaveAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory(StorageDirectory);
        await File.WriteAllTextAsync(ProfileFilePath, json, cancellationToken).ConfigureAwait(false);
    }

    // Saves the profile photo to the storage directory. It generates a file name based on the original file name and saves the photo.
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

    //retrieves the full path to the profile photo if it exists. 
    public string? GetPhotoPath(string? photoFileName)
    {
        if (string.IsNullOrWhiteSpace(photoFileName))
            return null;

        var path = Path.Combine(StorageDirectory, photoFileName);
        return File.Exists(path) ? path : null;
    }
}
