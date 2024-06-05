using System.Net;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using Polly;
using Polly.Retry;
using RestEase;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Services;

public sealed class BotFactoryService(IBotFactoryClient client, ILogger logger) : IBotFactoryService
{
    private string _token = string.Empty;

    private AsyncRetryPolicy CreateWaitAndRetryPolicy(IEnumerable<TimeSpan> sleepsBeetweenRetries,
                                                      Func<ApiException, bool> condition,
                                                      string logMessage)
    {
        return Policy
                .Handle(condition)
                .WaitAndRetryAsync(
                    sleepDurations: sleepsBeetweenRetries,
                    onRetry: (ex, span, retryCount, _) =>
                    {
                        logger.Warning("Retrying: {LogMessage}", logMessage);
                    });
    }

    public async Task<bool> CreateChatbotAsync(CreateChatbotRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) ], 
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Forbidden), 
                                                       $"Create chatbot for {request.FullName}");

            await retryPolicy.ExecuteAsync(() =>
                client.CreateChatbotAsync(_token, request)
            );

            return true;
        }
        catch (ApiException restEx)
        {
            // BotFactory returns 401, but chatbot is created on Blip Portal anyway.
            var created = restEx.StatusCode.Equals(HttpStatusCode.Forbidden);

            if (!created)
            {
                logger.Error("Rest Error to Create Group Chatbot: {Name}", request.FullName);
            }

            return created;
        }
    }

    public async Task<bool> CreateQueuesAsync(string chatbotShortName, CreateQueuesRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60) ], 
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created), 
                                                       $"Creating queues for {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() => 
                client.CreateQueuesAsync(_token, chatbotShortName, request)
            );

            return true;
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to create queues for Dealer: {ChatbotShortName}", chatbotShortName);
            return false;
        }
    }

    public async Task<bool> CreateRulesAsync(string chatbotShortName, CreateRulesRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) ], 
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created), 
                                                       $"Creating rules for {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() => 
                client.CreateRulesAsync(_token, chatbotShortName, request)
            );

            return true;
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to create queue rules for {ChatbotShortName}", chatbotShortName);
            return false;
        }
    }

    public async Task<IEnumerable<Application>> GetAllApplicationsAsync(string tenantId)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) ], 
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.OK), 
                                                       $"Getting all applications for tenantId {tenantId}");

            var applications = await retryPolicy.ExecuteAsync(() => 
                client.GetAllAplicationsAsync(_token, tenantId)
            );

            return applications.Results;
        }
        catch (Exception ex)
        {
            logger.Error("Error to get chatbots for tenantId {TenantId}: {ErrorMessage}", tenantId, ex.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Queue>> GetAllQueuesAsync(string chatbotShortName)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) ], 
                                                       ex => ex.Content is not null && !ex.Content.Contains("there are no saved queues"), 
                                                       $"Getting {chatbotShortName} queues");

            var queues = await retryPolicy.ExecuteAsync(() => 
                client.GetAllQueuesAsync(_token, chatbotShortName)
            );

            return queues.Results;
        }
        catch (ApiException restEx)
        {
            if (restEx.Content is not null && restEx.Content.Contains("there are no saved queues"))
                return [];

            logger.Error("RestEase Error to get chatbot group queues: {ShortName}", chatbotShortName);
            return null;
        }
    }

    public void SetToken(string token)
    {
        _token = token;
    }
}