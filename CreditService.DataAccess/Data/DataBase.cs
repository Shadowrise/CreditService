using CreditService.Infrastructure.Database;

namespace CreditService.Infrastructure.Data;

class DataBase
{
    protected IDatabase Database { get; set; }
    
    protected DataBase(IDatabase database)
    {
        Database = database;
    }
}