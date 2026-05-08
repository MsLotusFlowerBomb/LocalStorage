using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ass5.Models;

[Table("Shopping_cart_items")]
public class ShoppingCartItem
{
    [Column("id")]
    public int Id { get; set; }

    [Column("profile_id")]
    public int ProfileId { get; set; }

    [Column("shopping_item_id")]
    public int ShoppingItemId { get; set; }

    [Column("quantity")]
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
