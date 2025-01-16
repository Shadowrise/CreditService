using CreditService.Application.Interfaces.Data;
using CreditService.Infrastructure.Data;
using CreditService.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace CreditService.Infrastructure;

public static class ServiceExtensions
{
    public static void AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IDatabase, SqliteDatabase>();
        
        services.AddSingleton<ICreditData, CreditData>();
    }
}