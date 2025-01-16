using CreditService.Application.Interfaces.Data;

namespace CreditService.API.Endpoints;

public static class CreditEndpoints
{
    public const string GetCreditsWithInvoicesUrl = "/creditsWithInvoices";
    public const string GetCreditsOverviewByStatusUrl = "/creditsOverviewByStatus";
    
    public static void AddCreditEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(GetCreditsWithInvoicesUrl, GetCreditsWithInvoices);
        app.MapGet(GetCreditsOverviewByStatusUrl, GetCreditsOverviewByStatus);
    }
    
    private static async Task<IResult> GetCreditsWithInvoices(ICreditData creditData, CancellationToken cancellationToken)
    {
        var result = await creditData.GetCreditsWithInvoices(cancellationToken);

        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.InternalServerError();
    }
    
    private static async Task<IResult> GetCreditsOverviewByStatus(ICreditData creditData, CancellationToken cancellationToken)
    {
        var result = await creditData.GetCreditsOverviewByStatus(cancellationToken);

        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.InternalServerError();
    }
}