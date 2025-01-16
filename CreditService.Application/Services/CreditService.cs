using CreditService.Application.Interfaces.Data;
using CreditService.Application.Interfaces.Services;

namespace CreditService.Application.Services;

public class CreditService: ICreditService
{
    private readonly ICreditData _creditData;

    public CreditService(ICreditData creditData)
    {
        _creditData = creditData;
    }
}