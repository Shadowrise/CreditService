using CreditService.Application.Interfaces.Data;
using CreditService.Domain.Entities;
using CreditService.Infrastructure.Database;
using Dapper;
using OperationResult;

namespace CreditService.Infrastructure.Data;

class CreditData: DataBase, ICreditData
{
    public CreditData(IDatabase database) : base(database)
    {
    }

    public async Task<Result<ICollection<Credit>>> GetCreditsWithInvoices(CancellationToken cancellationToken)
    {
        using var con = Database.CreateConnection();
        
        var query = new CommandDefinition("""
            SELECT C.CreditID,
                   C.CreditNumber, 
                   C.ClientName, 
                   C.RequestedAmount, 
                   C.CreditRequestDate, 
                   C.CreditStatus,
                   I.CreditID,
                   I.InvoiceNumber,
                   I.InvoiceAmount
            FROM Credit C
            LEFT JOIN Invoice I ON C.CreditID = I.CreditID
        """, cancellationToken: cancellationToken);
        
        var creditsDict = new Dictionary<string, Credit>();
        await con.QueryAsync<Credit, Invoice, Credit>(
            query,
            (credit, invoice) =>
            {
                if (!creditsDict.TryGetValue(credit.CreditNumber, out var creditEntry))
                {
                    creditEntry = credit;
                    creditsDict.Add(creditEntry.CreditNumber, creditEntry);
                }

                if (invoice.InvoiceNumber != null)
                {
                    creditEntry.Invoices.Add(invoice);   
                }
                
                return credit;
            },
            splitOn: "CreditID");
        
        return creditsDict.Values;
    }

    public async Task<Result<CreditsOverviewByStatus>> GetCreditsOverviewByStatus(CancellationToken cancellationToken)
    {
        using var con = Database.CreateConnection();
        
        var query = new CommandDefinition("""
            SELECT 
                SUM(CASE WHEN CreditStatus = 'Paid' THEN RequestedAmount ELSE 0 END) AS TotalPaid,
                SUM(CASE WHEN CreditStatus = 'AwaitingPayment' THEN RequestedAmount ELSE 0 END) AS TotalAwaitingPayment,
                (SUM(CASE WHEN CreditStatus = 'Paid' THEN RequestedAmount ELSE 0 END) * 100.0 / 
                 SUM(CASE WHEN CreditStatus IN ('Paid', 'AwaitingPayment') THEN RequestedAmount ELSE 0 END)) AS PercentagePaid,
                (SUM(CASE WHEN CreditStatus = 'AwaitingPayment' THEN RequestedAmount ELSE 0 END) * 100.0 / 
                 SUM(CASE WHEN CreditStatus IN ('Paid', 'AwaitingPayment') THEN RequestedAmount ELSE 0 END)) AS PercentageAwaitingPayment
            FROM Credit
            WHERE CreditStatus IN ('Paid', 'AwaitingPayment');
        """, cancellationToken: cancellationToken);
        
        var data = await con.QueryFirstAsync<CreditsOverviewByStatus>(query);

        return data;
    }
}