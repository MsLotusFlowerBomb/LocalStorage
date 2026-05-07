using System.Text.Json.Serialization;

namespace Ass5.Models;

public sealed class ShoppingItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("stock_quantity")]
    public int StockQuantity { get; set; }
}
