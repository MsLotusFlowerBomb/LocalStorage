using System.Text.Json.Serialization;

namespace Ass5.Models;

public sealed class ShoppingCartItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("profile_id")]
    public int ProfileId { get; set; }

    [JsonPropertyName("shopping_item_id")]
    public int ShoppingItemId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonIgnore]
    public string ItemName { get; set; } = string.Empty;

    [JsonIgnore]
    public decimal UnitPrice { get; set; }

    [JsonIgnore]
    public int StockQuantity { get; set; }

    [JsonIgnore]
    public decimal Total => UnitPrice * Quantity;
}
