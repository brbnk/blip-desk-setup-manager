using Blip.Dealer.Desk.Manager.Models.Google;

namespace Blip.Dealer.Desk.Manager.Services;

public interface IGoogleSheetsService
{
    public GoogleSheetsService SetSpreadSheetId(string spreadSheetId);

    public Task<IEnumerable<T>> ReadAsync<T>(string sheetName, string columnRange) where T : GoogleSheet;

    public  Task<IEnumerable<IGrouping<string, DealerSetupSheet>>> ReadAndGroupDealersAsync(string spreadSheetId, string sheetName, string range, string brand);

    public  Task<IEnumerable<DealerSetupSheet>> ReadDealersAsync(string spreadSheetId, string sheetName, string range, string brand);
}