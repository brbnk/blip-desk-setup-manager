using Blip.Dealer.Desk.Manager.Facade;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Serilog;
using Take.Blip.Client.Extensions;

namespace Blip.Dealer.Desk.Manager.Facades.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependecyInjection(this IServiceCollection services)
    {
        services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
        services.AddScoped<IDealerSetupFacade, DealerSetupFacade>();
        services.AddScoped<IBotFactoryService, BotFactoryService>();
        services.AddScoped<IServiceHourFacade, ServiceHourFacade>();
        services.AddScoped<ITagsFacade, TagsFacade>();
        services.AddScoped<ICustomRepliesFacade, CustomRepliesFacade>();

        services.AddSingleton<IBlipClientFactory, BlipClientFactory>();

        return services;
    }

    public static IServiceCollection AddRestEaseClients(this IServiceCollection services)
    {
        services.AddSingleton(RestClient.For<IBotFactoryClient>("https://mcmh01bt-55598.brs.devtunnels.ms/"));

        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        services.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger());

        return services;
    }

    public static IServiceCollection AddBlipSerializer(this IServiceCollection services)
    {
        var documentResolver = new DocumentTypeResolver();
        
        documentResolver.WithBlipDocuments();
        
        var envelopeSerializer = new EnvelopeSerializer(documentResolver);

        services.AddSingleton(envelopeSerializer);

        return services;
    }
}