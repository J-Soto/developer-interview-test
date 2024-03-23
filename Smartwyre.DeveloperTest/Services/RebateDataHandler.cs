using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

public class RebateDataHandler
{
    private IRebateDataStore _rebateDataStore;
    public RebateDataHandler() { }
    public RebateDataHandler(IRebateDataStore rebateDataStore)
    {
        _rebateDataStore = rebateDataStore;
    }
    public List<Rebate> GetAllRebates()
    {
        return _rebateDataStore.GetAllRebates();
    }
    public virtual Rebate GetRebate(string rebateIdentifier)
    {
        return _rebateDataStore.GetRebate(rebateIdentifier);
    }
    public virtual void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
    }
    public void InsertTestRebate()
    {
        _rebateDataStore.InsertTestRebate();
    }
    public void InsertRebate(Rebate rebate)
    {
        _rebateDataStore.InsertRebate(rebate);
    }
}