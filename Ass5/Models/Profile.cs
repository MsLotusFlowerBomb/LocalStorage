using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ass5.Models;

[Table("profiles")]
public class Profile: BaseModel
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("surname")]
    public string? Surname { get; set; }

    [Column("email_address")]
    public string? EmailAddress { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }
}
