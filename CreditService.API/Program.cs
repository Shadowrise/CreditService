using CreditService.API.Endpoints;
using CreditService.Application;
using CreditService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDataAccess();
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.AddCreditEndpoints();

await app.Services.CreateDatabase();

app.Run();

public partial class Program { }