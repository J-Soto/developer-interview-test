using Smartwyre.DeveloperTest.Types;

public class FixedCashAmountIncentive : IIncentive
{
    private Rebate rebate;

    public FixedCashAmountIncentive(Rebate rebate)
    {
        this.rebate = rebate;
    }

    public decimal Calculate(CalculateRebateRequest request)
    {
        return rebate.Amount;
    }
}