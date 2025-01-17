using CreditService.Domain.Enums;

namespace CreditService.Infrastructure.Models;

public class CreditWithInvoiceRow
{
    public string CreditNumber { get; init; }
    public string ClientName { get; init; }
    public int RequestedAmount { get; init; }
    public DateTime CreditRequestDate { get; init; }
    public CreditStatus CreditStatus { get; init; }
    
    public string? InvoiceNumber { get; init; }
    public int? InvoiceAmount { get; init; }
}