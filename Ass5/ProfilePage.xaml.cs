using Ass5.Utilities;
using Ass5.ViewModels;

namespace Ass5;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<ProfileViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnChangePhotoClicked(object? sender, EventArgs e)
    {
        _viewModel.StatusMessage = "Photo upload is not connected to Supabase profile storage in this assignment build.";
        await DisplayAlert("Photo", "Photo upload is not connected to Supabase profile storage in this assignment build.", "OK");
    }
}
