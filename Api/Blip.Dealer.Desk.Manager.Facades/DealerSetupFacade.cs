using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Enums;
using Blip.Dealer.Desk.Manager.Models.Google;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;
using Queue = Blip.Dealer.Desk.Manager.Models.Blip.Queue;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class DealerSetupFacade(IGoogleSheetsService googleSheetsService,
                                      IBotFactoryService botFactoryService,
                                      ILogger logger) : IDealerSetupFacade
{
    private readonly IList<string> _groups = [];
    private IEnumerable<Application> _applications = [];

    public async Task PublishDealerSetupAsync(PublishDealerSetupRequest request)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);
        
        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId, 
                                                                request.DataSource.Name, 
                                                                request.DataSource.Range, 
                                                                request.Brand);

        var tasks = new List<Func<Task>>();

        foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, request.ImageUrl);

            tasks.Add(() => HandleChatbotCreationAsync(chatbot, dealer));
        }

        foreach (var task in tasks)
        {
            await task();
        }

        logger.Information("Dealers setup publish completed!");
    }

    private async Task HandleChatbotCreationAsync(Chatbot chatbot, DealerSetupSheet dealers)
    {
        var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

        var nameWithSuffix = chatbot.NameWithSuffix;

        var state = _groups.Contains(nameWithSuffix) || application is not null ? 
            ChatbotState.EXISTS : ChatbotState.NEW;

        var shortName = application is not null ? application.ShortName : nameWithSuffix;

        if (state.Equals(ChatbotState.NEW)) 
        {
            var createRequest = new CreateChatbotRequest(chatbot.Tenant, nameWithSuffix, chatbot.FullName, chatbot.ImageUrl);

            var created = await botFactoryService.CreateChatbotAsync(createRequest);

            if (!created) 
                return;

            _groups.Add(shortName);
        }
        else
        {
            logger.Warning("Group chatbot already exists {FullName}", chatbot.FullName);
        }

        await HandleQueuesCreation(shortName);
    }

    private async Task HandleQueuesCreation(string chatbotShortName)
    {
        var queues = await botFactoryService.GetAllQueuesAsync(chatbotShortName);

        if (queues is null)
            return;

        var defaultQueueExist = queues.Any(q => q.Name.Equals("Default"));

        if (defaultQueueExist)
        {
            logger.Warning("Default queue for {ChatbotShortName} already exists", chatbotShortName);
            return;
        }

        var defaultQueue = new Queue("Default");
        var defaultQueueRequest = new CreateQueuesRequest() { 
            Queues = [ defaultQueue.Name ]
        };

        var defaultQueueSuccess = await botFactoryService.CreateQueuesAsync(chatbotShortName, defaultQueueRequest);

        if (defaultQueueSuccess)
        {
            logger.Information("Success to create default queue for {ChatbotShortName}", chatbotShortName);
        }
        else 
        {
            logger.Error("Error to create default queue for {ChatbotShortName}", chatbotShortName);
        }
    }
}