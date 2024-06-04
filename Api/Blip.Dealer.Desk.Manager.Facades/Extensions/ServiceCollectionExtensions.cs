using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using Microsoft.Extensions.DependencyInjection;
using RestEase;

namespace Blip.Dealer.Desk.Manager.Facades.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependecyInjection(this IServiceCollection service)
    {
        service.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
        service.AddScoped<IDeskManagerFacade, DeskManagerFacade>();
        service.AddScoped<IBotFactoryService, BotFactoryService>();

        return service;
    }

    public static IServiceCollection AddRestEaseClients(this IServiceCollection service)
    {
        service.AddSingleton(RestClient.For<IBotFactoryClient>("https://botfactory.cs.blip.ai"));

        return service;
    }
}