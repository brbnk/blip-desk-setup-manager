using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blip.Dealer.Desk.Manager.Controllers;

[ApiController]
[Route("[controller]")]
public class DeskManagerController(IDeskManagerFacade deskManagerFacade) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PublishDealerSetupAsync([FromHeader] string token, 
                                                             [FromBody] PublishDealerSetupRequest request)
    {
        request.SetBearerToken(token);

        var response = await deskManagerFacade.PublishDealerSetupAsync(request);

        return Ok(response);
    }
}