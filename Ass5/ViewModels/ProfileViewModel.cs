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
            OnProfileChanged();
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
            OnProfileChanged();
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
            OnProfileChanged();
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
            OnProfileChanged();
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

        _profile = new Profile { Id = ProfileId };
        RaiseAllProperties();
        await SaveAsync();
    }

    private void RaiseAllProperties()
    {
        OnProfileChanged();
        OnPropertyChanged(nameof(ProfileId));
    }

    private void OnProfileChanged()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Surname));
        OnPropertyChanged(nameof(EmailAddress));
        OnPropertyChanged(nameof(Bio));
    }
}
