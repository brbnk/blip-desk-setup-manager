using Blip.Dealer.Desk.Manager.Facades;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facade;

public sealed class TagsFacade(IBotFactoryService botFactoryService,
                               IGoogleSheetsService googleSheetsService,
                               ILogger logger) : ITagsFacade
{
    private IEnumerable<Application> _applications = [];
    
    public async Task PublishTagsAsync(PublishTagsRequest request)
    {
        logger.Information("Starting to create Tags...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

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
            
            tasks.Add(HandleTagsPublishAsync(application.ShortName, request.Tags));
        }

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Tags publishing completed!");
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup, string tenant)
    {
        var name =  $"{brand.Trim().ToUpper()} - {dealerGroup}";
        
        return new Chatbot(name, tenant, imageUrl: "");
    }

    private async Task HandleTagsPublishAsync(string shortName, IList<string> newTags)
    {
        if (!newTags.Any())
        {
            logger.Warning("Skiping tags creation for {Dealer} because its empty", shortName);
            return;
        }

        var tags = await botFactoryService.GetTagsAsync(shortName);

        var onlyNewTags = new List<string>();

        foreach (var nt in newTags)
        {
            if (tags.Any(t => t.Equals(nt.ToLower().Trim()))) 
            {
                logger.Warning("Tag {Tag} already exists on {Dealer}", nt, shortName);
            }
            else
            {
                onlyNewTags.Add(nt);
            }
        }

        if (onlyNewTags.Count == 0)
        {
            logger.Warning("Empty tags after removing duplicates. Skipping tag creation for {Dealer}", shortName);
            return;
        }

        var tagsRequest = new CreateTagsRequest()
        {
            Tags = onlyNewTags.Select(tag => new Tag { Text = tag }).ToList()
        };

        await botFactoryService.CreateTagsAsync(shortName, tagsRequest);
    } 
}