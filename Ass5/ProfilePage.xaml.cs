using Ass5.Models;
using Ass5.Services;

namespace Ass5;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileStorage _storage;
    private Profile _profile = new();

    public ProfilePage()
    {
        InitializeComponent();
        _storage = new ProfileStorage();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        StatusLabel.Text = $"Profile file: {ProfileStorage.ProfileFilePath}";
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            _profile = await _storage.LoadAsync();

            NameEntry.Text = _profile.Name;
            SurnameEntry.Text = _profile.Surname;
            EmailEntry.Text = _profile.EmailAddress;
            BioEditor.Text = _profile.Bio;

            UpdatePhotoPreview();
            StatusLabel.Text = $"Profile file: {ProfileStorage.ProfileFilePath}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Load failed: {ex.Message}";
        }
    }

    private void UpdatePhotoPreview()
    {
        var path = _storage.GetPhotoPath(_profile.PhotoFileName);
        ProfileImage.Source = path is null ? "dotnet_bot.png" : ImageSource.FromFile(path);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            _profile.Name = NameEntry.Text?.Trim();
            _profile.Surname = SurnameEntry.Text?.Trim();
            _profile.EmailAddress = EmailEntry.Text?.Trim();
            _profile.Bio = BioEditor.Text;

            await _storage.SaveAsync(_profile);
            StatusLabel.Text = $"Saved to: {ProfileStorage.ProfileFilePath}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Save failed: {ex.Message}";
        }
    }

    private async void OnChoosePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo is null)
                return;

            var fileName = await _storage.SavePhotoAsync(photo);
            _profile.PhotoFileName = fileName;

            await _storage.SaveAsync(_profile);
            UpdatePhotoPreview();
            StatusLabel.Text = "Photo saved.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Photo failed: {ex.Message}";
        }
    }

    private async void OnClearPhotoClicked(object? sender, EventArgs e)
    {
        _profile.PhotoFileName = null;
        UpdatePhotoPreview();

        try
        {
            await _storage.SaveAsync(_profile);
        }
        catch
        {
            // ignore
        }
    }
}
