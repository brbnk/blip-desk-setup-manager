using Blip.Dealer.Desk.Manager.Models.AppSettings;
using Blip.Dealer.Desk.Manager.Models.Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Services;

public class GoogleSheetsService : IGoogleSheetsService
{
  private SheetsService _sheetsService => Authenticate();
  private readonly GoogleSheetsSettings _googleSheetsSettings;
  private string _spreadSheetId;
  private readonly ILogger _logger;

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

  public GoogleSheetsService(ILogger logger, IOptions<GoogleSheetsSettings> options)
  {
    _googleSheetsSettings = options.Value;
    _logger = logger;
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

  public async Task<IEnumerable<IGrouping<string, DealerSetupSheet>>> ReadAndGroupDealersAsync(string spreadSheetId, string sheetName, string range, string brand)
  {
    try
    {
      var dealers = await SetSpreadSheetId(spreadSheetId).ReadAsync<DealerSetupSheet>(sheetName, range);

      if (!dealers.Any())
      {
        _logger.Warning("Sheet is empty");
        
        throw new Exception("Sheet is empty");
      }

      var groups = dealers.Where(d => !string.IsNullOrWhiteSpace(d.Code) && brand.Equals(d.Brand))
                          .GroupBy(d => d.Group);

      return groups;
    }
    catch (Exception ex)
    {
      _logger.Error("Unable to read Dealers Setup Sheet: {ErrorMessage}", ex.Message);
      throw;
    }
  }

  #region Private Methods

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

  #endregion
}