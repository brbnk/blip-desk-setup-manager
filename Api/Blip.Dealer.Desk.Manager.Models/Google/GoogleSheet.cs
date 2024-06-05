namespace Blip.Dealer.Desk.Manager.Models.Google;

public abstract record GoogleSheet
{
  private const string CSV_DATA_SEPARATOR = ",";

  public override string ToString()
  {
    var properties = GetType()
        .GetProperties()
        .Select(p => EscapeLineBreaker((string)p.GetValue(this)));

    return string.Join(CSV_DATA_SEPARATOR, properties);
  }

  private static string EscapeLineBreaker(string text) =>
    text.Replace("\n", "\\n");
}