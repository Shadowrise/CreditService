using CreditService.API.Endpoints;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CreditService.API.Tests.Integration;

public class CreditEndpointsTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CreditEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetCreditsWithInvoices_Ok()
    {
        // Arrange
        var url = CreditEndpoints.GetCreditsWithInvoicesUrl;
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetCreditsOverviewByStatus_Ok()
    {
        // Arrange
        var url = CreditEndpoints.GetCreditsOverviewByStatusUrl;
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}