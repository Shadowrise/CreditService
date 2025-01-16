using System.Data;

namespace CreditService.Infrastructure.Database;

public interface IDatabase
{
    IDbConnection CreateConnection();
    Task Init();
    Task Seed();
}