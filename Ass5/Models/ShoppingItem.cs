using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ass5.Models;

[Table("shopping_items")]
public  class ShoppingItem : BaseModel
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("stock_quantity")]
    public int StockQuantity { get; set; }
}
