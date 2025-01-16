using CreditService.Domain.Entities;
using OperationResult;

namespace CreditService.Application.Interfaces.Data;

public interface ICreditData
{
    /// <summary>
    /// All credits with information about the invoices for each credit
    /// </summary>
    Task<Result<ICollection<Credit>>> GetCreditsWithInvoices(CancellationToken cancellationToken);
    
    /// <summary>
    /// The total sum of credits with the "Paid" status, the total
    /// sum of credits with the "AwaitingPayment" status, and the percentage of each of
    /// these amounts in relation to the total sum of all credits with the statuses "Paid"
    /// and "AwaitingPayment".
    /// </summary>
    Task<Result<CreditsOverviewByStatus>> GetCreditsOverviewByStatus(CancellationToken cancellationToken);
}