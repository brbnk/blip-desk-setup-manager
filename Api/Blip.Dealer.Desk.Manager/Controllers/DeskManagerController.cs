using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blip.Dealer.Desk.Manager.Controllers;

[ApiController]
[Route("[controller]")]
public class DeskManagerController(IDeskManagerFacade deskManagerFacade) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PublishDealerDeskSetupAsync([FromBody] PublishDealerSetupRequest request)
    {
        var response = await deskManagerFacade.ReadGoogleSheetAsync(request);

        return Ok(response);
    }
}