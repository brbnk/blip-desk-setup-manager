using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models;
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
    private string _tenant = string.Empty;
    private ReportSheet _reportSheet;
    private readonly IList<ReportSheet> _report = [];

    public async Task<IList<ReportSheet>> PublishDealerSetupAsync(PublishDealerSetupRequest request)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _tenant = request.Tenant;

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

        var tasks = new List<Func<Task>>();

        foreach (var group in groups)
        {
            _reportSheet = new();

            var dealerGroup = group.Key;

            var chatbot = SetupChatbot(request.Brand, dealerGroup, request.ImageUrl);

            tasks.Add(() => HandleChatbotCreationAsync(chatbot, group));
        }

        foreach (var task in tasks)
        {
            await task();
        }

        return _report;
    }

    private Chatbot SetupChatbot(string brand, string dealerGroup, string imageUrl = "")
    {
        var name =  $"{brand.Trim().ToUpper()} - {dealerGroup}";
        
        return new Chatbot(name, _tenant, imageUrl);
    }

    private async Task HandleChatbotCreationAsync(Chatbot chatbot, IGrouping<string, DealerSetupSheet?> dealers)
    {
        var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

        var nameWithSuffix = chatbot.NameWithSuffix;

        var state = _groups.Contains(nameWithSuffix) || application is not null ? 
            ChatbotState.EXISTS : ChatbotState.NEW;

        var shortName = application is not null ? application.ShortName : nameWithSuffix;

        _reportSheet.SetBotId(shortName);

        if (state.Equals(ChatbotState.NEW)) 
        {
            var createRequest = new CreateChatbotRequest(chatbot.Tenant, nameWithSuffix, chatbot.FullName, chatbot.ImageUrl);

            var created = await botFactoryService.CreateChatbotAsync(createRequest);

            if (!created) 
                return;

            _groups.Add(shortName);
            
            // Publish flow
        }
        else
        {
            logger.Warning("Group chatbot already exists {FullName}", chatbot.FullName);
        }
        
        _reportSheet.SetChatbotStepStatus(success: true);

        await HandleQueuesCreation(shortName, dealers);
    }

    private async Task HandleQueuesCreation(string chatbotShortName, IGrouping<string, DealerSetupSheet?> dealers)
    {
        var queues = await botFactoryService.GetAllQueuesAsync(chatbotShortName);

        if (queues is null)
            return;

        var request = new CreateQueuesRequest();
        var rulesRequest = new CreateRulesRequest();

        foreach (var dealer in dealers)
        {
            var newQueue = new Queue(dealer?.FantasyName);

            var queueExists = queues.Any(q => 
            {
                var queue = new Queue(q.Name);
                return queue.NormalizedName.Equals(newQueue.NormalizedName) || request.Queues.Any(q => q.Equals(newQueue.Name));
            });

            if (queueExists) 
            {
                _reportSheet.SetQueuesStepStatus(success: true);
                _reportSheet.SetRulesStepStatus(success: true);

                _report.Add(CreateSheetIntance(_reportSheet));

                logger.Warning("Queue already exists: {QueueName}", dealer?.FantasyName);
                continue;
            }

            request.Queues.Add(newQueue.Name);

            var rule = new Rule(newQueue.Name, dealer?.Code);

            rulesRequest.Rules.Add(rule);
        }

        if (!request.Queues.Any()) 
        {
            _reportSheet.SetRulesStepStatus(success: true);
            logger.Warning("Empty list after remove existing queues: {ChabotShortName}", chatbotShortName);
            return;
        }

        var success = await botFactoryService.CreateQueuesAsync(chatbotShortName, request);

        if (success)
        {
            _reportSheet.SetQueuesStepStatus(success: true);
            await HandleRulesCreation(chatbotShortName, rulesRequest);
        }
    }

    private async Task HandleRulesCreation(string chatbotShortName, CreateRulesRequest request)
    {
        var success = await botFactoryService.CreateRulesAsync(chatbotShortName, request);

        if (success) 
        {
            _reportSheet.SetRulesStepStatus(success: true);
            logger.Information("Setup was configured with success! {ChabotShortName}", chatbotShortName);
        }

        _report.Add(CreateSheetIntance(_reportSheet));
    }

    public static ReportSheet CreateSheetIntance(ReportSheet sheet)
    {
        var newSheet = new ReportSheet(); 

        newSheet.SetBotId(sheet.BotId);
        newSheet.SetChatbotStepStatus(success: sheet.ChatbotStepStatus.Equals("OK"));
        newSheet.SetQueuesStepStatus(success: sheet.QueuesStepStatus.Equals("OK"));
        newSheet.SetRulesStepStatus(success: sheet.RulesStepStatus.Equals("OK"));

        return newSheet;
    }
}