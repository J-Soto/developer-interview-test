/*
@workspace Ahora mira las definiciones de las clases e interfaces relacionadas, incluyendo ProductDataHandler, RebateDataHandler, IIncentive, CalculateRebateRequest, CalculateRebateResult, y cualquier otra clase o interfaz que se utilice en RebateService.
 */

//using Smartwyre.DeveloperTest.Data;
using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private ProductDataHandler productDataHandler;
    private RebateDataHandler rebateDataHandler;
    private IIncentiveFactory incentiveFactory;

    public RebateService(ProductDataHandler productDataHandler, RebateDataHandler rebateDataHandler, IIncentiveFactory incentiveFactory)
    {
        this.productDataHandler = productDataHandler;
        this.rebateDataHandler = rebateDataHandler;
        this.incentiveFactory = incentiveFactory;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {

        /* 
        var rebateDataStore = new RebateDataStore();
        var productDataStore = new ProductDataStore();

        
        Rebate rebate = rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();
        */

        /*
        var result = new CalculateRebateResult();
        Product product = productDataHandler.GetProduct(request.ProductIdentifier);
        Rebate rebate = rebateDataHandler.GetRebate(request.RebateIdentifier);

        */

        //var rebateAmount = 0m;

        /* 
        switch (rebate.Incentive)
        {
            case IncentiveType.FixedCashAmount:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
                {
                    result.Success = false;
                }
                else if (rebate.Amount == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount = rebate.Amount;
                    result.Success = true;
                }
                break;

            case IncentiveType.FixedRateRebate:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (product == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
                {
                    result.Success = false;
                }
                else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount += product.Price * rebate.Percentage * request.Volume;
                    result.Success = true;
                }
                break;

            case IncentiveType.AmountPerUom:
                if (rebate == null)
                {
                    result.Success = false;
                }
                else if (product == null)
                {
                    result.Success = false;
                }
                else if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
                {
                    result.Success = false;
                }
                else if (rebate.Amount == 0 || request.Volume == 0)
                {
                    result.Success = false;
                }
                else
                {
                    rebateAmount += rebate.Amount * request.Volume;
                    result.Success = true;
                }
                break;
        }
        */

        /*/
        if (product == null || rebate == null)
        {
            result.Success = false;
            return result;
        }

        IIncentive incentive = incentiveFactory.CreateIncentive(rebate, product);

        if (incentive == null)
        {
            result.Success = false;
            return result;
        }
        //rebateAmount = incentive.Calculate(request);
        result.Success = true;

        if (result.Success)
        {
            rebateDataHandler.StoreCalculationResult(rebate, incentive.Calculate(request));
        }

        return result;
        */

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "The request cannot be null.");
        }

        var (product, rebate) = GetProductAndRebate(request);
        return CalculateRebate(request, product, rebate);
    }

    private (Product, Rebate) GetProductAndRebate(CalculateRebateRequest request)
    {
        var product = productDataHandler.GetProduct(request.ProductIdentifier) ?? throw new ProductNotFoundException($"Product with ID {request.ProductIdentifier} not found");
        var rebate = rebateDataHandler.GetRebate(request.RebateIdentifier) ?? throw new RebateNotFoundException($"Rebate with ID {request.RebateIdentifier} not found");
        return (product, rebate);
    }

    private CalculateRebateResult CalculateRebate(CalculateRebateRequest request, Product product, Rebate rebate)
    {
        var result = new CalculateRebateResult
        {
            Success = false
        };
        IIncentive incentive = incentiveFactory.CreateIncentive(rebate, product) ?? throw new IncentiveCreationFailedException($"Failed to create incentive for product ID {product.Id} and rebate ID {rebate.Identifier}");
        result.Success = true;
        result.Amount = incentive.Calculate(request);
        rebateDataHandler.StoreCalculationResult(rebate, result.Amount);
        return result;
    }
}