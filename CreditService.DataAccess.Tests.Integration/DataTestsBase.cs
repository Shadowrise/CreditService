using System.Data;
using CreditService.Infrastructure.Database;
using Microsoft.Extensions.Configuration;

namespace CreditService.DataAccess.Tests.Integration;

public class DataTestsBase: IAsyncLifetime
{
    protected IDatabase Database { get; private set; }
    protected IDbConnection Connection { get; private set; }
    
    public async Task InitializeAsync()
    {
        var myConfiguration = new Dictionary<string, string>
        {
            {"ConnectionStrings:CreditServiceDb", "Data Source=InMemorySample;Mode=Memory;Cache=Shared"},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration!)
            .Build();
        
        Database = new SqliteDatabase(configuration);
        Connection = Database.CreateConnection();
        
        await Database.Init();
    }

    public Task DisposeAsync()
    {
        Connection.Dispose();
        return Task.CompletedTask;
    }
}