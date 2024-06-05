namespace Blip.Dealer.Desk.Manager.Models.Blip;

public sealed partial class Chatbot(string name, string tenant, string imageUrl) : BlipEntity
{
    public override int MaxLength => 30;
    
    public string ImageUrl { get; set; } = imageUrl;

    public string Tenant { get; set; } = tenant;

    public string ShortName => RemoveWhiteSpacesRegex().Replace(FullName, "").ToLower();
    
    public string FullName => Normalize(name);

    public string NameWithSuffix => $"{ShortName}{Suffix}";

    public static string Suffix => Guid.NewGuid().ToString()[..5];
}