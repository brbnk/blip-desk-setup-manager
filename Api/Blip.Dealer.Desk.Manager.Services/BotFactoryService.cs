using System.Net;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using Blip.Dealer.Desk.Manager.Services.Extensions;
using Polly;
using Polly.Retry;
using RestEase;
using Serilog;
using Blip.Dealer.Desk.Manager.Models;

namespace Blip.Dealer.Desk.Manager.Services;

public sealed class BotFactoryService(IBotFactoryClient client, ILogger logger) : IBotFactoryService
{
    private string _token = string.Empty;

    private readonly IEnumerable<TimeSpan> _intervals = [
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

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
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
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
            var created = restEx.StatusCode == HttpStatusCode.Forbidden;

            if (created)
            {
                logger.Information("Success to Create Group Chatbot {Name}: {Message}", request.FullName, restEx.Content);
            }
            else 
            {
                logger.Error("Rest Error to Create Group Chatbot {Name}: {Message}", request.FullName, restEx.Content);
            }

            return created;
        }
        catch (Exception excpetion)
        {
            logger.Error("Exception Error to Create Group Chatbot: {Name}", excpetion.Message);
            return false;
        }
    }

    public async Task<bool> CreateQueuesAsync(string chatbotShortName, CreateQueuesRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created),
                                                       $"Creating queues for {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() =>
                client.CreateQueuesAsync(_token, chatbotShortName, request)
            );

            return true;
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to create queues for Dealer: {ChatbotShortName}: {ErrorMessage}", chatbotShortName, restEx.Content);
            return false;
        }
    }

    public async Task<bool> CreateRulesAsync(string chatbotShortName, CreateRulesRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created),
                                                       $"Creating rules for {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() =>
                client.CreateRulesAsync(_token, chatbotShortName, request)
            );

            return true;
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to create queue rules for {ChatbotShortName}: {ErrorMessage}", chatbotShortName, restEx.Content);
            return false;
        }
    }

    public async Task CreateAttendantsAsync(string chatbotShortName, CreateAttendantsRequest request)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created),
                                                       $"Creating attendats for {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() =>
                client.CreateAttendantsAsync(_token, chatbotShortName, request)
            );

            logger.Information("Success to create attendants for {Dealer}", chatbotShortName);
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to create attendants for {Dealer}: {ErrorMessage}", chatbotShortName, restEx.Content);
        }
    }

    public async Task<IEnumerable<Application>> GetAllApplicationsAsync(string tenantId)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
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
            throw;
        }
    }

    public async Task<Application> GetApplicationAsync(string shortName)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.OK),
                                                       $"Getting access key for chatbot {shortName}");

            var application = await retryPolicy.ExecuteAsync(() =>
                client.GetAplicationAsync(_token, shortName)
            );

            return application;                            
        }
        catch (Exception ex)
        {
            logger.Error("Error to get access key for chabot {Chatbot}: {ErrorMessage}", shortName, ex.Message);
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetTagsAsync(string shortName)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.OK),
                                                       $"Getting tags for chatbot {shortName}");

            var application = await retryPolicy.ExecuteAsync(() =>
                client.GetTagsASync(_token, shortName)
            );

            return application.Results;                            
        }
        catch (Exception ex)
        {
            logger.Error("Error to get tags for chabot {Chatbot}: {ErrorMessage}", shortName, ex.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Queue>> GetAllQueuesAsync(string chatbotShortName)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => ex.Content is not null && !ex.Content.Contains(Constants.QUEUE_NOT_EXIST_MESSAGE),
                                                       $"Getting {chatbotShortName} queues");

            var queues = await retryPolicy.ExecuteAsync(() =>
                client.GetAllQueuesAsync(_token, chatbotShortName)
            );

            return queues.Results;
        }
        catch (ApiException restEx)
        {
            if (restEx.Content is not null && restEx.Content.Contains(Constants.QUEUE_NOT_EXIST_MESSAGE))
                return [];

            logger.Error("RestEase Error to get chatbot group queues: {ShortName}", chatbotShortName);
            return null;
        }
    }

    public async Task PublishFlowAsync(string chatbotShortName, Stream file)
    {
        try
        {
            var retryPolicy = CreateWaitAndRetryPolicy(_intervals,
                                                       ex => !ex.StatusCode.Equals(HttpStatusCode.Created),
                                                       $"Publishing flow async {chatbotShortName}");

            await retryPolicy.ExecuteAsync(() =>
                client.PublishFlowAsync(file, _token, chatbotShortName)
            );

            logger.Information("Success to publish flow for {Dealer}", chatbotShortName);
        }
        catch (ApiException restEx)
        {
            logger.Error("Error to publish flow for {Dealer}: {ErrorMessage}", chatbotShortName, restEx.Content);
        }
    }

    public void SetToken(string token)
    {
        _token = token;
    }
}