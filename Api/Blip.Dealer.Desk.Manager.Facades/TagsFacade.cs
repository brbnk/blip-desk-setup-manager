using Blip.Dealer.Desk.Manager.Facades;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facade;

public sealed class TagsFacade(IBotFactoryService botFactoryService,
                               IBlipCommandService blipCommandService,
                               IBlipClientFactory blipClientFactory,
                               IGoogleSheetsService googleSheetsService,
                               ILogger logger) : ITagsFacade
{
    private IEnumerable<Application> _applications = [];
    
    public async Task PublishTagsAsync(PublishTagsRequest request)
    {
        logger.Information("Starting to create Tags...");

        botFactoryService.SetToken(request.GetBearerToken());

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId, 
                                                                 request.DataSource.Name, 
                                                                 request.DataSource.Range, 
                                                                 request.Brand);

        var tasks = new List<Func<Task>>();

        foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null) 
            {
                logger.Warning("Chatbot does not exist: {Group}", dealer.FantasyName);
                continue;
            }
            
            tasks.Add(() => HandleTagsPublishAsync(application, request.Tags));
        }

        foreach (var task in tasks)
        {
            await task();
        }

        logger.Information("Tags publishing completed!");
    }

    private async Task HandleTagsPublishAsync(Application application, IList<string> newTags)
    {
        if (!newTags.Any())
        {
            logger.Warning("Skiping tags creation for {Dealer} because its empty", application.ShortName);
            return;
        }

        var tags = await botFactoryService.GetTagsAsync(application.ShortName);

        var tagsToSend = tags.ToList();

        foreach (var nt in newTags)
        {
            if (tagsToSend.Exists(t => nt.ToLower().Trim().Equals(t.ToLower().Trim())))
            {
                logger.Warning("Tag {Tag} already exists on {Dealer}", nt, application.ShortName);
            }
            else
            {
                tagsToSend.Add(nt);
            }
        }

        if (tagsToSend.Count == 0)
        {
            logger.Warning("Empty tags after removing duplicates. Skipping tag creation for {Dealer}", application.ShortName);
            return;
        }

        // Get AccessKey
        var chatbot = await botFactoryService.GetApplicationAsync(application.ShortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;

        var tagsRequest = tagsToSend.Select(tag => new Tag { Text = tag }).ToList();

        await blipCommandService.PublishTagsAsync(application.ShortName, botAuthKey, tagsRequest);
    } 
}