using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

public interface IRebateDataStore
{
    Rebate GetRebate(string rebateIdentifier);
    void StoreCalculationResult(Rebate account, decimal rebateAmount);
    List<Rebate> GetAllRebates();
    void InsertTestRebate();
    void InsertRebate(Rebate rebate);
}