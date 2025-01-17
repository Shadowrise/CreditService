namespace CreditService.Infrastructure.Mappers;

static class CurrencyMapper
{
    internal static decimal FromDb(int value)
    {
        return value / 100M;
    }
    
    internal static int ToDb(decimal value)
    {
        return (int)(value * 100M);
    }
}