using Smartwyre.DeveloperTest.Types;

public interface IIncentiveFactory
{
    IIncentive CreateIncentive(Rebate rebate, Product product);
}