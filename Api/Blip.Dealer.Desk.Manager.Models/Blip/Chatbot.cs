using System.Text.RegularExpressions;

namespace Blip.Dealer.Desk.Manager.Models.Blip;

public sealed partial class Chatbot(string name) 
{
    private const int MaxLength = 30;
    
    public string Id => RemoveWhiteSpacesRegex().Replace(Name, "").ToLower();
    
    public string Name => Normalize(name);

    private static string Normalize(string name)
    {
        var trimed = name.Trim();

        var withNoSpecialChars = RemoveSpecialCharsRegex().Replace(trimed, "");

        var withSingleWhiteSpaces = RemoveMultipleWhiteSpacesRegex().Replace(withNoSpecialChars, " ");

        if (withNoSpecialChars.Length > MaxLength) 
        {
            return withSingleWhiteSpaces[..MaxLength];
        }

        return withSingleWhiteSpaces;
    }

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex RemoveMultipleWhiteSpacesRegex();

    [GeneratedRegex(@"[^0-9a-zA-Z\s]+")]
    private static partial Regex RemoveSpecialCharsRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex RemoveWhiteSpacesRegex();
}