using CreditService.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreditService.Application;

public static class ServiceExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICreditService, Services.CreditService>();
    }
}