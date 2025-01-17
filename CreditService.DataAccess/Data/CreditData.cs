using CreditService.Application.Interfaces.Data;
using CreditService.Domain.Entities;
using CreditService.Infrastructure.Database;
using CreditService.Infrastructure.Mappers;
using CreditService.Infrastructure.Models;
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
            SELECT C.CreditNumber, 
                   C.ClientName, 
                   C.RequestedAmount, 
                   C.CreditRequestDate, 
                   C.CreditStatus,
                   I.InvoiceNumber,
                   I.InvoiceAmount
            FROM Credit C
            LEFT JOIN Invoice I ON C.CreditID = I.CreditID
        """, cancellationToken: cancellationToken);
        
        var rows = await con.QueryAsync<CreditWithInvoiceRow>(query);
        
        var creditsDict = new Dictionary<string, Credit>();
        foreach (var row in rows)
        {
            if (!creditsDict.TryGetValue(row.CreditNumber, out var credit))
            {
                credit = new Credit()
                {
                    CreditNumber = row.CreditNumber,
                    ClientName = row.ClientName,
                    RequestedAmount = CurrencyMapper.FromDb(row.RequestedAmount),
                    CreditRequestDate = row.CreditRequestDate,
                    CreditStatus = row.CreditStatus
                };
                creditsDict.Add(credit.CreditNumber, credit);
            }

            if (row.InvoiceNumber != null)
            {
                credit.Invoices.Add(new Invoice()
                {
                    InvoiceNumber = row.InvoiceNumber,
                    InvoiceAmount = CurrencyMapper.FromDb(row.InvoiceAmount!.Value),
                });   
            }
        }
        
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
        
        var rows = await con.QueryFirstAsync<CreditsOverviewByStatusRow>(query);
        var creditsOverviewByStatus = new CreditsOverviewByStatus()
        {
            TotalPaid = CurrencyMapper.FromDb(rows.TotalPaid),
            TotalAwaitingPayment = CurrencyMapper.FromDb(rows.TotalAwaitingPayment),
            PercentagePaid = Math.Round(rows.PercentagePaid, 2),
            PercentageAwaitingPayment = Math.Round(rows.PercentageAwaitingPayment, 2)
        };

        return creditsOverviewByStatus;
    }
}