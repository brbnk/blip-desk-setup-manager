using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Serilog;

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
        service.AddSingleton(RestClient.For<IBotFactoryClient>("https://419fsdbf-55598.brs.devtunnels.ms/"));

        return service;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection service)
    {
        service.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger());

        return service;
    }
}