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
                ('CR-001', 'Alice Johnson', 5000.00, '2025-01-01', 'Created'),
                ('CR-002', 'Bob Smith', 10000.00, '2025-01-02', 'AwaitingPayment')
        """);
        
        await con.ExecuteAsync("""
            INSERT INTO Invoice (InvoiceNumber, InvoiceAmount, CreditID) VALUES
                ('INV-001', 1000.00, 1),
                ('INV-002', 2000.00, 1)
        """);
        
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsWithInvoices(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        
        var cr1 = result.Value.FirstOrDefault(x => x.CreditNumber == "CR-001");
        Assert.NotNull(cr1);
        Assert.Equal(2, cr1.Invoices.Count);
        
        var cr2 = result.Value.FirstOrDefault(x => x.CreditNumber == "CR-002");
        Assert.NotNull(cr2);
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
        Assert.Equal(0, result.Value.TotalPaid);
        Assert.Equal(0, result.Value.TotalAwaitingPayment);
        Assert.Equal(0, result.Value.PercentagePaid);
        Assert.Equal(0, result.Value.PercentageAwaitingPayment);
    }
    
    [Fact]
    public async Task GetCreditsOverviewByStatus_DataExistsInDb_ResultIsCorrect()
    {
        // Arrange
        using var con = Database.CreateConnection();
        await con.ExecuteAsync("""
            INSERT INTO Credit (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatus) VALUES
                ('CR-001', 'Alice Johnson', 1000.00, '2025-01-01', 'Paid'),
                ('CR-002', 'Bob Smith', 2000.00, '2025-01-02', 'Paid'),
                ('CR-003', 'Charlie Brown', 3000.00, '2025-01-03', 'AwaitingPayment'),
                ('CR-004', 'Diana Prince', 4000.00, '2025-01-04', 'AwaitingPayment')
        """);
        
        await con.ExecuteAsync("""
            INSERT INTO Invoice (InvoiceNumber, InvoiceAmount, CreditID) VALUES
                ('INV-001', 1000.00, 1),
                ('INV-002', 2000.00, 1)
        """);
        
        // Arrange
        var target = new CreditData(Database);
        
        // Act
        var result = await target.GetCreditsOverviewByStatus(CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3000M, result.Value.TotalPaid);
        Assert.Equal(7000M, result.Value.TotalAwaitingPayment);
        Assert.Equal(30M, result.Value.PercentagePaid);
        Assert.Equal(70M, result.Value.PercentageAwaitingPayment);
    }
}