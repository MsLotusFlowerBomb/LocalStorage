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

    // When the page appears, it loads the profile data and updates the UI.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        StatusLabel.Text = $"Profile file: {ProfileStorage.ProfileFilePath}";
        await LoadAsync();
    }

    // Loads the profile data from storage and updates the UI fields. If loading fails, it shows an error message.

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

    // Handles the "Change Photo" button click. It prompts the user to take a photo or choose one from the gallery, saves the photo.
    private async void OnChangePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var action = await DisplayActionSheet("Profile photo", "Cancel", null, "Take Photo", "Choose Photo");
            if (action is null || action == "Cancel")
                return;

            FileResult? photo = null;

            if (action == "Take Photo")
                photo = await MediaPicker.Default.CapturePhotoAsync();
            else if (action == "Choose Photo")
                photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo is null)
                return;

            var fileName = await _storage.SavePhotoAsync(photo);
            _profile.PhotoFileName = fileName;

            await _storage.SaveAsync(_profile);
            UpdatePhotoPreview();
            StatusLabel.Text = $"Photo saved to: {ProfileStorage.StorageDirectory}";
        }
        catch (FeatureNotSupportedException)
        {
            StatusLabel.Text = "Camera/photo picker not supported on this device.";
        }
        catch (PermissionException)
        {
            StatusLabel.Text = "Permission denied for camera/photos.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Photo failed: {ex.Message}";
        }
    }
    // Updates the profile photo preview. If a photo is set, it loads it from storage; otherwise, it shows a default image.

    private void UpdatePhotoPreview()
    {
        var path = _storage.GetPhotoPath(_profile.PhotoFileName);
        ProfileImage.Source = path is null ? "picc.png" : ImageSource.FromFile(path);
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
            var message = $"Profile saved.\n\nFile location:\n{ProfileStorage.ProfileFilePath}";
            StatusLabel.Text = $"Saved to: {ProfileStorage.ProfileFilePath}";
            await DisplayAlert("Saved", message, "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Save failed: {ex.Message}";
        }
    }

    //Clears all text fields and Profile picture
    private async void OnClearClicked(object? sender, EventArgs e)
    {
        NameEntry.Text = string.Empty;
        SurnameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        BioEditor.Text = string.Empty;

        _profile = new Profile();
        UpdatePhotoPreview();

        try
        {
            await _storage.SaveAsync(_profile);
            StatusLabel.Text = "Cleared.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Clear failed: {ex.Message}";
        }
    }
}
