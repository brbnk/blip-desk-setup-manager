using System.Text.RegularExpressions;

namespace Blip.Dealer.Desk.Manager.Models.Blip;

public abstract partial class BlipEntity
{
    public abstract int MaxLength { get; }

    public string Normalize(string name)
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
    public static partial Regex RemoveMultipleWhiteSpacesRegex();

    [GeneratedRegex(@"[^0-9a-zA-Z\s]+")]
    public static partial Regex RemoveSpecialCharsRegex();

    [GeneratedRegex(@"\s+")]
    public static partial Regex RemoveWhiteSpacesRegex();
}