using CreditService.Infrastructure.Data;
using Dapper;

namespace CreditService.DataAccess.Tests.Integration;

public class CreditDataTests: DataTestsBase
{
    [Fact]
    public async Task GetCreditsWithInvoices_NoDataInDb_EmptyResult()
    {
        // Arrange
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsWithInvoices(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
    
    [Fact]
    public async Task GetCreditsWithInvoices_DataExistsInDb_ResultIsCorrect()
    {
        // Arrange
        using var con = Database.CreateConnection();
        await con.ExecuteAsync("""
            INSERT INTO Credit (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus) VALUES
                ('CR-001', 'Alice Johnson', 500050, '2025-01-01', 'Created'),
                ('CR-002', 'Bob Smith', 1000010, '2025-01-02', 'AwaitingPayment')
        """);
        
        await con.ExecuteAsync("""
            INSERT INTO Invoice (InvoiceNumber, InvoiceAmount, CreditID) VALUES
                ('INV-001', 100010, 1),
                ('INV-002', 200020, 1)
        """);
        
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsWithInvoices(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        
        Assert.Contains(result.Value, x => x.CreditNumber == "CR-001");
        var cr1 = result.Value.First(x => x.CreditNumber == "CR-001");
        Assert.Equal(5000.50M, cr1.RequestedAmount);
        Assert.Equal(2, cr1.Invoices.Count);
        Assert.Contains(cr1.Invoices, x => x.InvoiceNumber == "INV-001" && x.InvoiceAmount == 1000.10M);
        Assert.Contains(cr1.Invoices, x => x.InvoiceNumber == "INV-002" && x.InvoiceAmount == 2000.20M);
        
        Assert.Contains(result.Value, x => x.CreditNumber == "CR-002");
        var cr2 = result.Value.First(x => x.CreditNumber == "CR-002");
        Assert.Equal(10000.10M, cr2.RequestedAmount);
        Assert.Empty(cr2.Invoices);
    }
    
    [Fact]
    public async Task GetCreditsOverviewByStatus_NoDataInDb_ZerosResult()
    {
        // Arrange
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsOverviewByStatus(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0M, result.Value.TotalPaid);
        Assert.Equal(0M, result.Value.TotalAwaitingPayment);
        Assert.Equal(0M, result.Value.PercentagePaid);
        Assert.Equal(0M, result.Value.PercentageAwaitingPayment);
    }
    
    [Fact]
    public async Task GetCreditsOverviewByStatus_DataExistsInDb_ResultIsCorrect()
    {
        // Arrange
        using var con = Database.CreateConnection();
        await con.ExecuteAsync("""
            INSERT INTO Credit (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus) VALUES
                ('CR-001', 'Alice Johnson', 100010, '2025-01-01', 'Paid'),
                ('CR-002', 'Bob Smith', 200020, '2025-01-02', 'Paid'),
                ('CR-003', 'Charlie Brown', 300030, '2025-01-03', 'AwaitingPayment'),
                ('CR-004', 'Diana Prince', 400040, '2025-01-04', 'AwaitingPayment')
        """);
        
        // Arrange
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsOverviewByStatus(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3000.30M, result.Value.TotalPaid);
        Assert.Equal(7000.70M, result.Value.TotalAwaitingPayment);
        Assert.Equal(30M, result.Value.PercentagePaid);
        Assert.Equal(70M, result.Value.PercentageAwaitingPayment);
    }
}