using System.Collections.ObjectModel;
using System.Windows.Input;
using Ass5.Models;
using Ass5.Services;

namespace Ass5.ViewModels;

public sealed class ShoppingListViewModel : BaseViewModel
{
    private readonly IShoppingDataService _dataService;
    private int _profileId;

    public ShoppingListViewModel(IShoppingDataService dataService)
    {
        _dataService = dataService;
        Items = [];
        LoadCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        AddToCartCommand = new Command<ShoppingItem>(async item => await AddToCartAsync(item), item => !IsBusy && item is not null);
        ViewCartCommand = new Command(async () => await Shell.Current.GoToAsync("ShoppingCartPage"), () => !IsBusy);
    }

    public ObservableCollection<ShoppingItem> Items { get; }

    public ICommand LoadCommand { get; }
    public ICommand AddToCartCommand { get; }
    public ICommand ViewCartCommand { get; }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = "Loading shopping items...";

            await _dataService.InitializeAsync();
            var profile = await _dataService.GetOrCreateProfileAsync();
            _profileId = profile.Id;

            var items = await _dataService.GetShoppingItemsAsync();
            Items.Clear();

            foreach (var item in items)
                Items.Add(item);

            StatusMessage = $"Loaded {Items.Count} shopping items.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)LoadCommand).ChangeCanExecute();
            ((Command<ShoppingItem>)AddToCartCommand).ChangeCanExecute();
            ((Command)ViewCartCommand).ChangeCanExecute();
        }
    }

    private async Task AddToCartAsync(ShoppingItem? item)
    {
        if (item is null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            var added = await _dataService.AddToCartAsync(_profileId, item.Id, 1);
            StatusMessage = added
                ? $"Added {item.Name} to your shopping cart."
                : $"Cannot add more {item.Name}; stock limit reached.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Add failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            ((Command)LoadCommand).ChangeCanExecute();
            ((Command<ShoppingItem>)AddToCartCommand).ChangeCanExecute();
            ((Command)ViewCartCommand).ChangeCanExecute();
        }
    }
}
