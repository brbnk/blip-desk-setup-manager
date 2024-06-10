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
            },
            FlowStr = "{\"identifier\":\"<SHORTNAME>\",\"accessKey\":\"<ACCESSKEY>\",\"messageReceivers\":[{\"state\":\"default\",\"type\":\"MessageReceiver\"}],\"notificationReceivers\":[{\"type\":\"DeskNotificationReceiver\"}],\"serviceProviderType\":\"ServiceProvider\",\"settings\":{\"flow\":{\"id\":\"2d372d8f-921d-4e4f-a646-4e14014b5f5b\",\"version\":1,\"states\":[{\"id\":\"onboarding\",\"root\":true,\"name\":\"Start\",\"inputActions\":[],\"input\":{\"bypass\":false},\"outputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"stateId\":\"onboarding\",\"stateName\":\"Start\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"onboarding\",\"#stateName\":\"Start\",\"#stateId\":\"onboarding\",\"#messageId\":\"{{input.message@id}}\"},\"category\":\"flow\",\"action\":\"Start\"}}],\"afterStateChangedActions\":[],\"outputs\":[{\"stateId\":\"welcome\",\"conditions\":[{\"source\":\"input\",\"comparison\":\"matches\",\"values\":[\".*\"]}]},{\"stateId\":\"fallback\"}]},{\"id\":\"fallback\",\"name\":\"Exceptions\",\"inputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"previousStateId\":\"{{state.previous.id}}\",\"previousStateName\":\"{{state.previous.name}}\",\"stateId\":\"{{state.id}}\",\"stateName\":\"{{state.name}}\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"fallback\",\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"},\"category\":\"flow\",\"action\":\"Exceptions\"}}],\"input\":{\"bypass\":true},\"outputActions\":[],\"afterStateChangedActions\":[],\"outputs\":[{\"stateId\":\"error\",\"conditions\":[{\"source\":\"input\",\"comparison\":\"matches\",\"values\":[\".*\"]}]},{\"stateId\":\"onboarding\"}]},{\"id\":\"welcome\",\"name\":\"Welcome\",\"inputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"previousStateId\":\"{{state.previous.id}}\",\"previousStateName\":\"{{state.previous.name}}\",\"stateId\":\"{{state.id}}\",\"stateName\":\"{{state.name}}\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"welcome\",\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"},\"category\":\"flow\",\"action\":\"Welcome\"}},{\"type\":\"SendMessage\",\"settings\":{\"id\":\"00000000-0000-0000-0000-000000000000\",\"type\":\"application/vnd.lime.chatstate+json\",\"content\":{\"state\":\"composing\",\"interval\":1000},\"metadata\":{\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"}}},{\"type\":\"SendMessage\",\"settings\":{\"id\":\"00000000-0000-0000-0000-000000000001\",\"type\":\"text/plain\",\"content\":\"Olá! {{contact.name}}!\\nSeja bem-vindo(a)!\",\"metadata\":{\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"}}}],\"input\":{\"bypass\":false},\"outputActions\":[],\"afterStateChangedActions\":[],\"outputs\":[{\"stateId\":\"desk:668c6deb-1a44-43b4-ba93-9f2ec93e0e4d\",\"typeOfStateId\":\"state\",\"conditions\":[{\"source\":\"input\",\"comparison\":\"exists\",\"values\":[]}]},{\"stateId\":\"fallback\"}]},{\"id\":\"error\",\"name\":\"Default error\",\"inputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"previousStateId\":\"{{state.previous.id}}\",\"previousStateName\":\"{{state.previous.name}}\",\"stateId\":\"{{state.id}}\",\"stateName\":\"{{state.name}}\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"error\",\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"},\"category\":\"flow\",\"action\":\"Default error\"}},{\"type\":\"SendMessage\",\"settings\":{\"id\":\"00000000-0000-0000-0000-000000000002\",\"type\":\"application/vnd.lime.chatstate+json\",\"content\":{\"state\":\"composing\",\"interval\":1000},\"metadata\":{\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"}}},{\"type\":\"SendMessage\",\"settings\":{\"id\":\"00000000-0000-0000-0000-000000000003\",\"type\":\"text/plain\",\"content\":\"Desculpe, não consegui entender!\",\"metadata\":{\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"}}}],\"input\":{\"bypass\":true},\"outputActions\":[],\"afterStateChangedActions\":[],\"outputs\":[{\"stateId\":\"onboarding\"}]},{\"id\":\"desk:668c6deb-1a44-43b4-ba93-9f2ec93e0e4d\",\"root\":false,\"name\":\"Customer service\",\"deskStateVersion\":\"3.0.0\",\"inputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"previousStateId\":\"{{state.previous.id}}\",\"previousStateName\":\"{{state.previous.name}}\",\"stateId\":\"{{state.id}}\",\"stateName\":\"{{state.name}}\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"desk:668c6deb-1a44-43b4-ba93-9f2ec93e0e4d\",\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"},\"category\":\"flow\",\"action\":\"Customer service\"}},{\"type\":\"ForwardToDesk\",\"conditions\":[],\"settings\":{}}],\"input\":{\"bypass\":false,\"conditions\":[{\"source\":\"context\",\"variable\":\"desk_forwardToDeskState_status\",\"comparison\":\"equals\",\"values\":[\"Success\"]}]},\"outputActions\":[],\"afterStateChangedActions\":[{\"type\":\"LeavingFromDesk\",\"conditions\":[],\"settings\":{}}],\"outputs\":[{\"conditions\":[{\"source\":\"context\",\"variable\":\"input.type\",\"comparison\":\"equals\",\"values\":[\"application/vnd.iris.ticket+json\"]},{\"source\":\"context\",\"variable\":\"input.content@status\",\"comparison\":\"equals\",\"values\":[\"ClosedAttendant\"]}],\"stateId\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\"},{\"conditions\":[{\"source\":\"context\",\"variable\":\"input.type\",\"comparison\":\"equals\",\"values\":[\"application/vnd.iris.ticket+json\"]},{\"source\":\"context\",\"variable\":\"input.content@status\",\"comparison\":\"equals\",\"values\":[\"ClosedClient\"]}],\"stateId\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\"},{\"conditions\":[{\"source\":\"context\",\"variable\":\"input.type\",\"comparison\":\"equals\",\"values\":[\"application/vnd.iris.ticket+json\"]},{\"source\":\"context\",\"variable\":\"input.content@status\",\"comparison\":\"equals\",\"values\":[\"ClosedClientInactivity\"]}],\"stateId\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\"},{\"conditions\":[{\"source\":\"context\",\"variable\":\"desk_forwardToDeskState_status\",\"comparison\":\"equals\",\"values\":[\"Error\"]}],\"stateId\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\",\"typeOfStateId\":\"state\"},{\"stateId\":\"desk:668c6deb-1a44-43b4-ba93-9f2ec93e0e4d\"}]},{\"id\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\",\"root\":false,\"name\":\"Testeeeee\",\"inputActions\":[{\"type\":\"TrackContactsJourney\",\"settings\":{\"previousStateId\":\"{{state.previous.id}}\",\"previousStateName\":\"{{state.previous.name}}\",\"stateId\":\"{{state.id}}\",\"stateName\":\"{{state.name}}\"}},{\"type\":\"TrackEvent\",\"settings\":{\"extras\":{\"stateId\":\"c6851223-c62f-4e7c-9bbf-2bc1acedf331\",\"#stateName\":\"{{state.name}}\",\"#stateId\":\"{{state.id}}\",\"#messageId\":\"{{input.message@id}}\",\"#previousStateId\":\"{{state.previous.id}}\",\"#previousStateName\":\"{{state.previous.name}}\"},\"category\":\"flow\",\"action\":\"Testeeeee\"}}],\"input\":{\"bypass\":false},\"outputActions\":[],\"afterStateChangedActions\":[],\"outputs\":[{\"stateId\":\"fallback\"}]}],\"configuration\":{\"builder:minimumIntentScore\":\"0.5\",\"builder:stateTrack\":\"true\",\"builder:#localTimeZone\":\"E. South America Standard Time\",\"builder:useTunnelOwnerContext\":\"true\",\"authorizationKey\":\"<AUTHORIZATIONKEY>\",\"gmt\":\"-3\"},\"inputActions\":[],\"outputActions\":[],\"type\":\"flow\"}},\"settingsType\":\"Settings\"}"
        };

        request.SetBearerToken(token);

        await flowFacade.PublishFlowAsync(request, flow.OpenReadStream());

        return Ok();
    }
}