namespace Smartwyre.DeveloperTest.Types;

public class Rebate
{
    public Rebate()
    {
    }
    public string Identifier { get; set; }
    public IncentiveType Incentive { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}
