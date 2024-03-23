using System;
using Smartwyre.DeveloperTest.Types;

public class IncentiveFactory : IIncentiveFactory
{
    public IIncentive CreateIncentive(Rebate rebate, Product product)
    {
        if (product.SupportedIncentives != (SupportedIncentiveType)rebate.Incentive) throw new InvalidOperationException("The incentive of the product and the rebate are not compatible.");
        return rebate.Incentive switch
        {
            IncentiveType.FixedCashAmount => new FixedCashAmountIncentive(rebate),
            IncentiveType.FixedRateRebate => new FixedRateRebateIncentive(rebate, product),
            IncentiveType.AmountPerUom => new AmountPerUomIncentive(rebate, product),
            _ => null,
        };
    }
}