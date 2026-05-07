using Ass5.Models;

namespace Ass5.Services;

public interface IShoppingDataService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<Profile> GetOrCreateProfileAsync(CancellationToken cancellationToken = default);
    Task SaveProfileAsync(Profile profile, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShoppingItem>> GetShoppingItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShoppingCartItem>> GetCartItemsAsync(int profileId, CancellationToken cancellationToken = default);
    Task<bool> AddToCartAsync(int profileId, int shoppingItemId, int quantity, CancellationToken cancellationToken = default);
    Task RemoveFromCartAsync(int profileId, int shoppingItemId, CancellationToken cancellationToken = default);
}
