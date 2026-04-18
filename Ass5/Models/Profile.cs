namespace Ass5.Models;


//This class will be used to store and retrieve the profile information
public sealed class Profile
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? EmailAddress { get; set; }
    public string? Bio { get; set; }

    public string? PhotoFileName { get; set; }
}
