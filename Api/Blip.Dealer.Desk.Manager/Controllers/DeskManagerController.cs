using System.Text;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blip.Dealer.Desk.Manager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskManagerController(IDealerSetupFacade deskManagerFacade,
                                   IServiceHourFacade serviceHourFacade) : ControllerBase
{
    
    [HttpPost("dealers-setup")]
    public async Task<IActionResult> PublishDealerSetupAsync([FromHeader] string token, 
                                                             [FromBody] PublishDealerSetupRequest request)
    {   
        request.SetBearerToken(token);

        var rows = await deskManagerFacade.PublishDealerSetupAsync(request);

        if (!rows.Any()) 
            return NoContent();

        var sb = new StringBuilder();
        
        sb.AppendLine("BotId,ChatbotStatus,QueuesStatus,RulesStatus");

        foreach (var row in rows)
        {
            sb.AppendLine($"{row.ToString()}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"{Guid.NewGuid()}.csv");
    }

    [HttpPost("dealers-service-hours")]
    public async Task<IActionResult> PublishDealerServiceHoursAsync([FromHeader] string token,
                                                                    [FromBody] PublishServiceHoursRequest request)
    {
        request.SetBearerToken(token);

        await serviceHourFacade.PublishDealersServiceHoursAsync(request);

        return Ok();
    }
}