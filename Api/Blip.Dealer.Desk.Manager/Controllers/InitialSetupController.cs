using System.Text;
using Blip.Dealer.Desk.Manager.Facades;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blip.Dealer.Desk.Manager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InitialSetupController(IDealerSetupFacade deskManagerFacade,
                                    IServiceHourFacade serviceHourFacade,
                                    ITagsFacade tagsFacade,
                                    ICustomRepliesFacade customRepliesFacade,
                                    IFlowFacade flowFacade) : ControllerBase
{

    [HttpPost("dealers")]
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

    [HttpPost("service-hours")]
    public async Task<IActionResult> PublishDealerServiceHoursAsync([FromHeader] string token,
                                                                    [FromBody] PublishServiceHoursRequest request)
    {
        request.SetBearerToken(token);

        await serviceHourFacade.PublishDealersServiceHoursAsync(request);

        return Ok();
    }

    [HttpPost("tags")]
    public async Task<IActionResult> PublishTagsAsync([FromHeader] string token,
                                                      [FromBody] PublishTagsRequest request)
    {
        request.SetBearerToken(token);

        await tagsFacade.PublishTagsAsync(request);

        return Ok();
    }

    [HttpPost("custom-reply")]
    public async Task<IActionResult> PublishCustomRepliesAsync([FromHeader] string token,
                                                               [FromBody] PublishCustomRepliesRequest request)
    {
        request.SetBearerToken(token);

        await customRepliesFacade.PublishCustomRepliesAsync(request);

        return Ok();
    }

    [HttpPost("flow")]
    public async Task<IActionResult> PublishFlowAsync([FromHeader] string token,
                                                      [FromHeader] string spreadSheetId,
                                                      [FromHeader] string sheetName,
                                                      [FromHeader] string brand,
                                                      [FromHeader] string tenantId,
                                                      IFormFile flow)
    {
        var request = new PublishFlowRequest() 
        {
            Brand = brand,
            Tenant = tenantId,
            DataSource = new GoogleSheetsRequest() 
            {
                SpreadSheetId = spreadSheetId,
                Name = sheetName
            }
        };

        request.SetBearerToken(token);

        await flowFacade.PublishFlowAsync(request, flow.OpenReadStream());

        return Ok();
    }
}