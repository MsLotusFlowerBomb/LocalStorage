using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ass5.Models;

namespace Ass5.Services;

public sealed class SupabaseShoppingDataService : IShoppingDataService
{
    private const string ProfilesTable = "profiles";
    private const string ItemsTable = "shopping_items";
    private const string CartTable = "shopping_cart_items";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;

    private readonly List<ShoppingItem> _localItems =
    [
        new() { Id = 1, Name = "Milk", Description = "2L Dairy Milk", Price = 3.99m, StockQuantity = 8 },
        new() { Id = 2, Name = "Bread", Description = "Whole Wheat Loaf", Price = 2.49m, StockQuantity = 10 },
        new() { Id = 3, Name = "Apples", Description = "Bag of 6 apples", Price = 5.20m, StockQuantity = 12 },
        new() { Id = 4, Name = "Eggs", Description = "Dozen large eggs", Price = 4.10m, StockQuantity = 9 },
        new() { Id = 5, Name = "Coffee", Description = "Ground coffee 340g", Price = 11.75m, StockQuantity = 6 }
    ];

    private readonly List<ShoppingCartItem> _localCartItems = [];
    private readonly Profile _localProfile = new() { Id = 1 };

    public SupabaseShoppingDataService(HttpClient httpClient, SupabaseOptions options)
    {
        _httpClient = httpClient;
        _options = options;

        if (_options.IsConfigured)
        {
            _httpClient.BaseAddress = new Uri($"{_options.Url.TrimEnd('/')}/rest/v1/");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("apikey", _options.AnonKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.AnonKey);
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
            return;

        var items = await GetAsync<List<ShoppingItem>>($"{ItemsTable}?select=*", cancellationToken) ?? [];
        if (items.Count > 0)
            return;

        var payload = JsonSerializer.Serialize(_localItems, JsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, ItemsTable)
        {
            Content = content
        };
        request.Headers.Add("Prefer", "return=minimal");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Profile> GetOrCreateProfileAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
            return _localProfile;

        var profiles = await GetAsync<List<Profile>>($"{ProfilesTable}?select=*&id=eq.1", cancellationToken) ?? [];
        if (profiles.Count > 0)
            return profiles[0];

        var profile = new Profile { Id = 1 };
        await SaveProfileAsync(profile, cancellationToken);
        return profile;
    }

    public async Task SaveProfileAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            _localProfile.Name = profile.Name;
            _localProfile.Surname = profile.Surname;
            _localProfile.EmailAddress = profile.EmailAddress;
            _localProfile.Bio = profile.Bio;
            return;
        }

        profile.Id = 1;
        var payload = JsonSerializer.Serialize(profile, JsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, ProfilesTable)
        {
            Content = content
        };

        request.Headers.Add("Prefer", "resolution=merge-duplicates,return=representation");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetShoppingItemsAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
            return _localItems.OrderBy(i => i.Name).ToList();

        var items = await GetAsync<List<ShoppingItem>>($"{ItemsTable}?select=*&order=name.asc", cancellationToken);
        return items ?? [];
    }

    public async Task<IReadOnlyList<ShoppingCartItem>> GetCartItemsAsync(int profileId, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            var cart = from cartItem in _localCartItems
                       join item in _localItems on cartItem.ShoppingItemId equals item.Id
                       where cartItem.ProfileId == profileId
                       select new ShoppingCartItem
                       {
                           Id = cartItem.Id,
                           ProfileId = cartItem.ProfileId,
                           ShoppingItemId = cartItem.ShoppingItemId,
                           Quantity = cartItem.Quantity,
                           ItemName = item.Name,
                           UnitPrice = item.Price,
                           StockQuantity = item.StockQuantity
                       };

            return cart.OrderBy(c => c.ItemName).ToList();
        }

        var cartItems = await GetAsync<List<ShoppingCartItem>>($"{CartTable}?select=*&profile_id=eq.{profileId}", cancellationToken) ?? [];
        var items = await GetShoppingItemsAsync(cancellationToken);

        foreach (var cart in cartItems)
        {
            var item = items.FirstOrDefault(i => i.Id == cart.ShoppingItemId);
            if (item is null)
                continue;

            cart.ItemName = item.Name;
            cart.UnitPrice = item.Price;
            cart.StockQuantity = item.StockQuantity;
        }

        return cartItems.Where(c => !string.IsNullOrWhiteSpace(c.ItemName)).OrderBy(c => c.ItemName).ToList();
    }

    public async Task<bool> AddToCartAsync(int profileId, int shoppingItemId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            return false;

        if (!_options.IsConfigured)
        {
            var item = _localItems.FirstOrDefault(i => i.Id == shoppingItemId);
            if (item is null)
                return false;

            var existing = _localCartItems.FirstOrDefault(c => c.ProfileId == profileId && c.ShoppingItemId == shoppingItemId);
            var existingQty = existing?.Quantity ?? 0;
            if (existingQty + quantity > item.StockQuantity)
                return false;

            if (existing is null)
            {
                _localCartItems.Add(new ShoppingCartItem
                {
                    Id = _localCartItems.Count == 0 ? 1 : _localCartItems.Max(c => c.Id) + 1,
                    ProfileId = profileId,
                    ShoppingItemId = shoppingItemId,
                    Quantity = quantity
                });
            }
            else
            {
                existing.Quantity += quantity;
            }

            return true;
        }

        var items = await GetShoppingItemsAsync(cancellationToken);
        var itemMatch = items.FirstOrDefault(i => i.Id == shoppingItemId);
        if (itemMatch is null)
            return false;

        var existingItems = await GetAsync<List<ShoppingCartItem>>($"{CartTable}?select=*&profile_id=eq.{profileId}&shopping_item_id=eq.{shoppingItemId}", cancellationToken) ?? [];
        var existing = existingItems.FirstOrDefault();
        var existingQty = existing?.Quantity ?? 0;

        if (existingQty + quantity > itemMatch.StockQuantity)
            return false;

        if (existing is null)
        {
            var payload = JsonSerializer.Serialize(new ShoppingCartItem
            {
                ProfileId = profileId,
                ShoppingItemId = shoppingItemId,
                Quantity = quantity
            }, JsonOptions);

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, CartTable)
            {
                Content = content
            };
            request.Headers.Add("Prefer", "return=minimal");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return true;
        }

        var patchPayload = JsonSerializer.Serialize(new { quantity = existingQty + quantity }, JsonOptions);
        using var patchContent = new StringContent(patchPayload, Encoding.UTF8, "application/json");
        using var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"{CartTable}?id=eq.{existing.Id}")
        {
            Content = patchContent
        };
        patchRequest.Headers.Add("Prefer", "return=minimal");

        using var patchResponse = await _httpClient.SendAsync(patchRequest, cancellationToken);
        patchResponse.EnsureSuccessStatusCode();

        return true;
    }

    public async Task RemoveFromCartAsync(int profileId, int shoppingItemId, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            _localCartItems.RemoveAll(c => c.ProfileId == profileId && c.ShoppingItemId == shoppingItemId);
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{CartTable}?profile_id=eq.{profileId}&shopping_item_id=eq.{shoppingItemId}");
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
    }
}
