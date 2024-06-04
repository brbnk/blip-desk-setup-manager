using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blip.Dealer.Desk.Manager.Facades.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependecyInjection(this IServiceCollection service)
    {
        service.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
        service.AddScoped<IDeskManagerFacade, DeskManagerFacade>();

        return service;
    }
}