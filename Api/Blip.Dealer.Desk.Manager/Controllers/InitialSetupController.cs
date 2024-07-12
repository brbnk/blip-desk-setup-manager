using Blip.Dealer.Desk.Manager.Facades;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Manager.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blip.Dealer.Desk.Manager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InitialSetupController(IDealerSetupFacade deskManagerFacade,
                                    IServiceHourFacade serviceHourFacade,
                                    ITagsFacade tagsFacade,
                                    ICustomRepliesFacade customRepliesFacade,
                                    IAttendantsFacade attendantsFacade,
                                    IFlowFacade flowFacade,
                                    IRouterFacade routerFacade) : ControllerBase
{

    [HttpPost("dealers")]
    public async Task<IActionResult> PublishDealerSetupAsync([FromHeader] string token,
                                                             [FromBody] PublishDealerSetupRequest request)
    {
        request.SetBearerToken(token);

        await deskManagerFacade.PublishDealerSetupAsync(request);

        return Ok();
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

    [HttpPost("attendants")]
    public async Task<IActionResult> PublishAttendantsAsync([FromHeader] string token,
                                                            [FromBody] PublishAttendantsRequest request)
    {
        request.SetBearerToken(token);

        await attendantsFacade.PublishAttendantsAsync(request);

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
            },
            FlowStr = ""
        };

        request.SetBearerToken(token);

        await flowFacade.PublishFlowAsync(request, flow.OpenReadStream());

        return Ok();
    }

    [HttpPost("router")]
    public async Task<IActionResult> GetRouterService([FromHeader] string token, 
                                                      [FromBody] RouterServicesRequest request)
    {
        request.SetBearerToken(token);

        var response = await routerFacade.GetRouterServicesAsync(request);

        return Ok(response);
    }
}