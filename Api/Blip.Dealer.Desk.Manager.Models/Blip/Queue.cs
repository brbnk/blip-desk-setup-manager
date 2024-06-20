namespace Blip.Dealer.Desk.Manager.Models.Blip;

public sealed class Queue(string name) : BlipEntity
{
    public override int MaxLength => 250;

    public string NormalizedName => RemoveWhiteSpacesRegex().Replace(Normalize(name), "").ToLower();
    
    public string Name => name;
}