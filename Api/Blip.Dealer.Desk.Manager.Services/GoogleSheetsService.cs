using Blip.Dealer.Desk.Manager.Models.AppSettings;
using Blip.Dealer.Desk.Manager.Models.Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Services;

public class GoogleSheetsService : IGoogleSheetsService
{
  private SheetsService _sheetsService => Authenticate();
  private readonly GoogleSheetsSettings _googleSheetsSettings;
  private string _spreadSheetId;

  private static readonly string[] _columns = [
    "A",
    "B",
    "C",
    "D",
    "E",
    "F",
    "G",
    "H",
    "I",
    "J",
    "K",
    "L",
    "M",
    "N",
    "O",
    "P",
    "Q",
    "R",
    "S",
    "T",
    "U",
    "V",
    "W",
    "X",
    "Y",
    "Z"
  ];

  public GoogleSheetsService(IOptions<GoogleSheetsSettings> options)
  {
    _googleSheetsSettings = options.Value;
  }

  public GoogleSheetsService SetSpreadSheetId(string spreadSheetId)
  {
    _spreadSheetId = spreadSheetId;
    return this;
  }

  public async Task<IEnumerable<T>> ReadAsync<T>(string sheetName, string columnRange) where T : GoogleSheet
  {
    var request = _sheetsService.Spreadsheets.Values
        .Get(_spreadSheetId, $"{sheetName}!{columnRange}");

    var rows = await request.ExecuteAsync();

    var sheet = new List<T>();

    foreach (var row in rows.Values)
    {
      var rowColumnMap = row
          .Select((value, index) => new { Key = _columns[index], Value = value })
          .ToDictionary(o => o.Key, o => o.Value);

      var data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(rowColumnMap));

      sheet.Add(data);
    }

    return sheet;
  }

  private SheetsService Authenticate()
  {
    var credentials = JsonConvert.SerializeObject(_googleSheetsSettings.Credentials);

    var scopes = new string[] {
      SheetsService.Scope.Spreadsheets
    };

    var googleCredential = GoogleCredential
        .FromJson(credentials)
        .CreateScoped(scopes);

    return new SheetsService(new()
    {
      HttpClientInitializer = googleCredential,
      ApplicationName = _googleSheetsSettings.ApplicationName
    });
  }
}