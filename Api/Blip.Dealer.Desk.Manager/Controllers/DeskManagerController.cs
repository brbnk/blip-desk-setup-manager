using System.Text;
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

        var rows = await deskManagerFacade.PublishDealerSetupAsync(request);

        if (!rows.Any()) 
            return NoContent();

        var sb = new StringBuilder();
        
        sb.AppendLine("Group,Dealer,BotId,ChatbotStatus,QueuesStatus,RulesStatus");

        foreach (var row in rows)
        {
            sb.AppendLine($"{row}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"{Guid.NewGuid()}.csv");
    }
}