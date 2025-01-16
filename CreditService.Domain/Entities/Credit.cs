using CreditService.Domain.Enums;

namespace CreditService.Domain.Entities;

public class Credit
{
    public required string CreditNumber { get; init; }
    public required string ClientName { get; init; }
    public required decimal RequestedAmount { get; init; }
    public required DateTime CreditRequestDate { get; init; }
    public required CreditStatus CreditStatus { get; init; }
    public ICollection<Invoice> Invoices { get; init; } = new List<Invoice>();
}