using Ass5.Utilities;
using Ass5.ViewModels;

namespace Ass5;

public partial class ShoppingListPage : ContentPage
{
    private readonly ShoppingListViewModel _viewModel;

    public ShoppingListPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<ShoppingListViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
