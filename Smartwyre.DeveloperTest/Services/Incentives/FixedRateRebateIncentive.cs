using Smartwyre.DeveloperTest.Types;

public class FixedRateRebateIncentive : IIncentive
{
    private Rebate rebate;
    private Product product;

    public FixedRateRebateIncentive(Rebate rebate, Product product)
    {
        this.rebate = rebate;
        this.product = product;
    }

    public decimal Calculate(CalculateRebateRequest request)
    {
        if (rebate == null || product == null || !product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) || rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
        {
            return 0m;
        }
        else
        {
            return product.Price * rebate.Percentage * request.Volume;
        }
    }
}