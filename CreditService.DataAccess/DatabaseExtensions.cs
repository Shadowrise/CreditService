using CreditService.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace CreditService.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task CreateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
        await db.Init();
        await db.Seed();
    }
}