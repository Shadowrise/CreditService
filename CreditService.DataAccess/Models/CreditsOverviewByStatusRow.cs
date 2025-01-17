namespace CreditService.Infrastructure.Models;

public class CreditsOverviewByStatusRow
{
    public int TotalPaid { get; init; }
    public int TotalAwaitingPayment { get; init; }
    public decimal PercentagePaid { get; init; }
    public decimal PercentageAwaitingPayment { get; init; }
}