using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Google;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class DeskManagerFacade(IGoogleSheetsService googleSheetsService) : IDeskManagerFacade
{
    public async Task<IEnumerable<DealerSetupSheet>> ReadGoogleSheetAsync(PublishDealerSetupRequest request)
    {
        var result = await googleSheetsService.SetSpreadSheetId(request.SpreadSheetId)
                                              .ReadAsync<DealerSetupSheet>(request.SheetName, request.Range);

        return result;
    }
}