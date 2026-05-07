using Ass5.Utilities;
using Ass5.ViewModels;

namespace Ass5;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ShoppingCartViewModel _viewModel;

    public ShoppingCartPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<ShoppingCartViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
