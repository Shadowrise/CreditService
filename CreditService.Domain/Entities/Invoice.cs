namespace CreditService.Domain.Entities;

public class Invoice
{
    public required string InvoiceNumber { get; init; }
    public required decimal InvoiceAmount { get; init; }
}