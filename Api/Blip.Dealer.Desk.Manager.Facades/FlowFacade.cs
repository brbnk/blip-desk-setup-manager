using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class FlowFacade(IGoogleSheetsService googleSheetsService,
                               IBotFactoryService botFactoryService,
                               IBlipClientFactory blipClientFactory,
                               ILogger logger) : IFlowFacade
{
    private IEnumerable<Application> _applications = [];
    
    public async Task PublishFlowAsync(PublishFlowRequest request, Stream file)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        if (_applications is null)
        {
            logger.Error("It was not possible to find application");
            throw new Exception("Error to get all applications");
        }
        
        var groups = await googleSheetsService.ReadAndGroupDealersAsync(request.DataSource.SpreadSheetId, 
                                                                        request.DataSource.Name, 
                                                                        request.DataSource.Range, 
                                                                        request.Brand);
                                                                        var tasks = new List<Task>();

        foreach (var group in groups)
        {
            var chatbot = SetupChatbot(request.Brand, group.Key, request.Tenant);

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null) 
            {
                logger.Warning("Chatbot does not exist: {Group}", group.Key);
                continue;
            }
            
            tasks.Add(HandleFlowPublishAsync(application.ShortName, file));
        }

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Tags publishing completed!");
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup, string tenant)
    {
        var name =  $"{brand.Trim().ToUpper()} - {dealerGroup}";
        
        return new Chatbot(name, tenant, imageUrl: "");
    }

    private async Task HandleFlowPublishAsync(string shortName, Stream file)
    {
        await botFactoryService.PublishFlowAsync(shortName, file);
    }
}