namespace Blip.Dealer.Desk.Manager.Models.Blip;

public sealed partial class Chatbot(string brand, string name, string tenant, string imageUrl) : BlipEntity
{
    public override int MaxLength => 30;

    public string Brand { get; set; } = brand;

    public string Name { get; set; } = name;
    
    public string ImageUrl { get; set; } = imageUrl;

    public string Tenant { get; set; } = tenant;

    public string ShortName => RemoveWhiteSpacesRegex().Replace(FullName, "").ToLower();
    
    public string FullName => Normalize($"{Brand?.Trim().ToUpper()} - {Name}");

    public string NameWithSuffix => $"{ShortName}{Suffix}";

    public static string Suffix => Guid.NewGuid().ToString()[..3];
}