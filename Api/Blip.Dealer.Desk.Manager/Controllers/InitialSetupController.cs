using Blip.Dealer.Desk.Manager.Facades;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models;
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
    public async Task<IActionResult> PublishDealerSetupAsync([FromBody] PublishDealerSetupRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        await deskManagerFacade.PublishDealerSetupAsync(request);

        return Ok();
    }

    [HttpPost("service-hours")]
    public async Task<IActionResult> PublishDealerServiceHoursAsync([FromBody]PublishServiceHoursRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        await serviceHourFacade.PublishDealersServiceHoursAsync(request);

        return Ok();
    }

    [HttpPost("tags")]
    public async Task<IActionResult> PublishTagsAsync([FromBody] PublishTagsRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        await tagsFacade.PublishTagsAsync(request);

        return Ok();
    }

    [HttpPost("custom-reply")]
    public async Task<IActionResult> PublishCustomRepliesAsync([FromBody] PublishCustomRepliesRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        await customRepliesFacade.PublishCustomRepliesAsync(request);

        return Ok();
    }

    [HttpPost("attendants")]
    public async Task<IActionResult> PublishAttendantsAsync([FromBody] PublishAttendantsRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        await attendantsFacade.PublishAttendantsAsync(request);

        return Ok();
    }

    [HttpPost("flow")]
    public async Task<IActionResult> PublishFlowAsync(IFormFile flow,
                                                      [FromHeader] string spreadSheetId = Constants.SPREADSHEET_ID,
                                                      [FromHeader] string sheetName = Constants.SHEET_NAME,
                                                      [FromHeader] string brand = Constants.BRAND,
                                                      [FromHeader] string tenantId = Constants.TENANT_ID)
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

        request.SetBearerToken(Constants.TOKEN);

        await flowFacade.PublishFlowAsync(request, flow.OpenReadStream());

        return Ok();
    }

    [HttpPost("router")]
    public async Task<IActionResult> GetRouterService([FromBody] RouterServicesRequest request)
    {
        request.SetBearerToken(Constants.TOKEN);

        var response = await routerFacade.GetRouterServicesAsync(request);

        return Ok(response);
    }
}