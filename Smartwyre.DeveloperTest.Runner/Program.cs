using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var product = new Product();
        var rebate = new Rebate();
        var productDataStore = new ProductDataHandler(ProductDataStore.Instance); ;
        var rebateDataStore = new RebateDataHandler(RebateDataStore.Instance);
        productDataStore.InsertTestProduct();
        rebateDataStore.InsertTestRebate();
        while (true)
        {
            Console.WriteLine("1. Retrieve product");
            Console.WriteLine("2. Retrieve rebate");
            Console.WriteLine("3. Retrieve all products");
            Console.WriteLine("4. Retrieve all rebates");
            Console.WriteLine("5. Generate rebate request");
            Console.WriteLine("6. Insert product");
            Console.WriteLine("7. Insert rebate");
            Console.WriteLine("8. Exit");


            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Enter product identifier:");
                    var retrievedProductIdentifier = Console.ReadLine();

                    var retrievedProduct = productDataStore.GetProduct(retrievedProductIdentifier);

                    if (retrievedProduct != null)
                    {
                        Console.WriteLine($"Identifier: {retrievedProduct.Identifier}, Price: {retrievedProduct.Price}, Uom: {retrievedProduct.Uom}, SupportedIncentives: {retrievedProduct.SupportedIncentives}");
                    }
                    else
                    {
                        Console.WriteLine("Product not found.");
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter rebate identifier:");
                    var retrievedRebateIdentifier = Console.ReadLine();

                    var retrievedRebate = rebateDataStore.GetRebate(retrievedRebateIdentifier);

                    if (retrievedRebate != null)
                    {
                        Console.WriteLine($"Identifier: {retrievedRebate.Identifier}, Incentive: {retrievedRebate.Incentive}, Amount: {retrievedRebate.Amount}, Percentage: {retrievedRebate.Percentage}");
                    }
                    else
                    {
                        Console.WriteLine("Rebate not found.");
                    }
                    break;

                case "3":
                    var allProducts = productDataStore.GetAllProducts();
                    foreach (var prod in allProducts)
                    {
                        Console.WriteLine($"Identifier: {prod.Identifier}, Price: {prod.Price}, Uom: {prod.Uom}, SupportedIncentives: {prod.SupportedIncentives}");
                    }
                    break;

                case "4":
                    var allRebates = rebateDataStore.GetAllRebates();
                    foreach (var reb in allRebates)
                    {
                        Console.WriteLine($"Identifier: {reb.Identifier}, Incentive: {reb.Incentive}, Amount: {reb.Amount}, Percentage: {reb.Percentage}");
                    }
                    break;

                case "5":
                    Console.WriteLine("Enter rebate identifier:");
                    var rebateIdentifier = Console.ReadLine();

                    Console.WriteLine("Enter product identifier:");
                    var productIdentifier = Console.ReadLine();

                    Console.WriteLine("Enter volume:");
                    var volume = decimal.Parse(Console.ReadLine());

                    var calculateRebateRequest = new CalculateRebateRequest
                    {
                        RebateIdentifier = rebateIdentifier,
                        ProductIdentifier = productIdentifier,
                        Volume = volume
                    };

                    var rebateService = new RebateService(productDataStore, rebateDataStore, new IncentiveFactory());
                    var calculateRebateResult = rebateService.Calculate(calculateRebateRequest);
                    if (calculateRebateResult.Success)
                    {
                        Console.WriteLine($"The result of the rebate calculation is:  {calculateRebateResult.Amount}"); break;
                    }
                    else
                    {
                        Console.WriteLine("Failed to calculate the rebate.");
                    }
                    break;

                case "6":
                    Console.WriteLine("Enter product details:");
                    Console.WriteLine("Identifier:");
                    var prodId = Console.ReadLine();
                    Console.WriteLine("Price:");
                    var productPrice = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Uom:");
                    var productUom = Console.ReadLine();
                    Console.WriteLine("Supported Incentives:");
                    Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(SupportedIncentiveType))));
                    var productSupportedIncentives = (SupportedIncentiveType)Enum.Parse(typeof(SupportedIncentiveType), Console.ReadLine());

                    var produ = new Product
                    {
                        Identifier = prodId,
                        Price = productPrice,
                        Uom = productUom,
                        SupportedIncentives = productSupportedIncentives
                    };
                    productDataStore.InsertProduct(produ);
                    Console.WriteLine("Product successfully inserted.");
                    break;

                case "7":
                    Console.WriteLine("Enter rebate details:");
                    Console.WriteLine("Identifier:");
                    var rebId = Console.ReadLine();
                    Console.WriteLine("Amount:");
                    var rebateAmount = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Percentage:");
                    var rebatePercentage = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Incentive:");
                    Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(IncentiveType))));
                    var rebateIncentive = (IncentiveType)Enum.Parse(typeof(IncentiveType), Console.ReadLine());

                    var reba = new Rebate
                    {
                        Identifier = rebId,
                        Incentive = rebateIncentive,
                        Amount = rebateAmount,
                        Percentage = rebatePercentage
                    };

                    rebateDataStore.InsertRebate(reba);
                    Console.WriteLine("Rebate successfully inserted");
                    break;
                case "8":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}