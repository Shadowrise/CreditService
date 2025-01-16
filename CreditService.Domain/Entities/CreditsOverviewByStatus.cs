namespace CreditService.Domain.Entities;

public class CreditsOverviewByStatus
{
    public required decimal TotalPaid { get; init; }
    public required decimal TotalAwaitingPayment { get; init; }
    public required decimal PercentagePaid { get; init; }
    public required decimal PercentageAwaitingPayment { get; init; }
}