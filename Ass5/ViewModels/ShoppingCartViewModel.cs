using System.Collections.ObjectModel;
using System.Windows.Input;
using Ass5.Models;
using Ass5.Services;

namespace Ass5.ViewModels;

public sealed class ShoppingCartViewModel : BaseViewModel
{
    private readonly IShoppingDataService _dataService;
    private int _profileId;

    public ShoppingCartViewModel(IShoppingDataService dataService)
    {
        _dataService = dataService;
        CartItems = [];

        LoadCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        RemoveItemCommand = new Command<ShoppingCartItem>(async item => await RemoveItemAsync(item), item => !IsBusy && item is not null);
    }

    public ObservableCollection<ShoppingCartItem> CartItems { get; }

    public ICommand LoadCommand { get; }
    public ICommand RemoveItemCommand { get; }

    public decimal CartTotal => CartItems.Sum(i => i.Total);

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Loading shopping cart...";

            var profile = await _dataService.GetOrCreateProfileAsync();
            _profileId = profile.Id;

            var items = await _dataService.GetCartItemsAsync(_profileId);
            CartItems.Clear();
            foreach (var item in items)
                CartItems.Add(item);

            OnPropertyChanged(nameof(CartTotal));
            StatusMessage = CartItems.Count == 0
                ? "Your shopping cart is empty."
                : $"Loaded {CartItems.Count} cart item(s).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Cart load failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)LoadCommand).ChangeCanExecute();
            ((Command<ShoppingCartItem>)RemoveItemCommand).ChangeCanExecute();
        }
    }

    private async Task RemoveItemAsync(ShoppingCartItem? item)
    {
        if (item is null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            await _dataService.RemoveFromCartAsync(_profileId, item.ShoppingItemId);
            CartItems.Remove(item);
            OnPropertyChanged(nameof(CartTotal));
            StatusMessage = $"Removed {item.ItemName} from cart.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Remove failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)LoadCommand).ChangeCanExecute();
            ((Command<ShoppingCartItem>)RemoveItemCommand).ChangeCanExecute();
        }
    }
}
