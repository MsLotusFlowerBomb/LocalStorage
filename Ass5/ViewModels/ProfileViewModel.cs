using System.Windows.Input;
using Ass5.Models;
using Ass5.Services;

namespace Ass5.ViewModels;

public sealed class ProfileViewModel : BaseViewModel
{
    private readonly IShoppingDataService _dataService;

    private Profile _profile = new();

    public ProfileViewModel(IShoppingDataService dataService)
    {
        _dataService = dataService;
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        ClearCommand = new Command(async () => await ClearAsync(), () => !IsBusy);
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public int ProfileId => _profile.Id;

    public string? Name
    {
        get => _profile.Name;
        set
        {
            if (_profile.Name == value)
                return;
            _profile.Name = value;
            OnPropertyChanged();
        }
    }

    public string? Surname
    {
        get => _profile.Surname;
        set
        {
            if (_profile.Surname == value)
                return;
            _profile.Surname = value;
            OnPropertyChanged();
        }
    }

    public string? EmailAddress
    {
        get => _profile.EmailAddress;
        set
        {
            if (_profile.EmailAddress == value)
                return;
            _profile.EmailAddress = value;
            OnPropertyChanged();
        }
    }

    public string? Bio
    {
        get => _profile.Bio;
        set
        {
            if (_profile.Bio == value)
                return;
            _profile.Bio = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Loading profile...";

            _profile = await _dataService.GetOrCreateProfileAsync();
            RaiseAllProperties();
            StatusMessage = "Profile loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Profile load failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)SaveCommand).ChangeCanExecute();
            ((Command)ClearCommand).ChangeCanExecute();
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Saving profile...";
            await _dataService.SaveProfileAsync(_profile);
            OnPropertyChanged(nameof(ProfileId));
            StatusMessage = "Profile saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)SaveCommand).ChangeCanExecute();
            ((Command)ClearCommand).ChangeCanExecute();
        }
    }

    private async Task ClearAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Clearing profile...";

            _profile = await _dataService.GetOrCreateProfileAsync();
            _profile.Name = string.Empty;
            _profile.Surname = string.Empty;
            _profile.EmailAddress = string.Empty;
            _profile.Bio = string.Empty;
            RaiseAllProperties();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Clear failed: {ex.Message}";
            return;
        }
        finally
        {
            IsBusy = false;
            ((Command)SaveCommand).ChangeCanExecute();
            ((Command)ClearCommand).ChangeCanExecute();
        }

        await SaveAsync();
    }

    private void RaiseAllProperties()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Surname));
        OnPropertyChanged(nameof(EmailAddress));
        OnPropertyChanged(nameof(Bio));
        OnPropertyChanged(nameof(ProfileId));
    }
}
