namespace Blip.Dealer.Desk.Manager.Models.AppSettings;

public sealed class GoogleSheetsSettings
{
  public string ApiKey { get; set; }

  public string ApiUrlBase { get; set; }

  public string GeocodingApiUrlBase { get; set; }

  public string ApplicationName { get; set; }

  public GoogleSheetsCredentials Credentials { get; set; }
}