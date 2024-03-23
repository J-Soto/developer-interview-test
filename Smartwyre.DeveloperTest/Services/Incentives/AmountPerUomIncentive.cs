using Smartwyre.DeveloperTest.Types;

public class AmountPerUomIncentive : IIncentive
{
    private Rebate rebate;
    private Product product;

    public AmountPerUomIncentive(Rebate rebate, Product product)
    {
        this.rebate = rebate;
        this.product = product;
    }

    public decimal Calculate(CalculateRebateRequest request)
    {
        if (rebate == null || product == null || !product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) || rebate.Amount == 0 || request.Volume == 0)
        {
            return 0m;
        }
        else
        {
            return rebate.Amount * request.Volume;
        }
    }
}