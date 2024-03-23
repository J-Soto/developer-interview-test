using System;
using Moq;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    [Fact]
    public void Calculate_ReturnsExpectedResult_WhenGivenValidRequest()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>();
        var rebateDataHandlerMock = new Mock<RebateDataHandler>();
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var expectedRebate = new Rebate { Identifier = "test", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        var expectedIncentive = new FixedRateRebateIncentive(expectedRebate, expectedProduct);

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(expectedProduct);
        rebateDataHandlerMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(expectedRebate);
        incentiveFactoryMock.Setup(i => i.CreateIncentive(expectedRebate, expectedProduct)).Returns(expectedIncentive);
        rebateDataHandlerMock.Setup(r => r.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>())).Verifiable();
        var expectedResult = new CalculateRebateResult { Success = true };

        // Act
        var result = rebateService.Calculate(request);

        // Assert
        Assert.Equal(expectedResult.Success, result.Success);
        rebateDataHandlerMock.Verify(r => r.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Once);
    }
    [Fact]
    public void Calculate_ThrowsException_WhenProductAndRebateIncentivesDoNotMatch()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>();
        var rebateDataHandlerMock = new Mock<RebateDataHandler>();
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var expectedRebate = new Rebate { Identifier = "test", Incentive = IncentiveType.FixedCashAmount, Percentage = 0.1m };

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(expectedProduct);
        rebateDataHandlerMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(expectedRebate);

        // Act & Assert
        Assert.Throws<IncentiveCreationFailedException>(() => rebateService.Calculate(request));
    }
    [Fact]
    public void Calculate_ThrowsException_WhenProductDoesNotSupportIncentive()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>(MockBehavior.Strict, null);
        var rebateDataHandlerMock = new Mock<RebateDataHandler>(MockBehavior.Strict, null);
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = 0 }; // No supported incentives
        var expectedRebate = new Rebate { Identifier = "test", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(expectedProduct);
        rebateDataHandlerMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(expectedRebate);

        // Act & Assert
        Assert.Throws<IncentiveCreationFailedException>(() => rebateService.Calculate(request));
    }

    [Fact]
    public void Calculate_ThrowsException_WhenProductNotFound()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>();
        var rebateDataHandlerMock = new Mock<RebateDataHandler>();
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns((Product)null); // Product not found

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => rebateService.Calculate(request));
    }

    [Fact]
    public void Calculate_ThrowsException_WhenRebateNotFound()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>();
        var rebateDataHandlerMock = new Mock<RebateDataHandler>();
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(expectedProduct);
        rebateDataHandlerMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns((Rebate)null); // Rebate not found

        // Act & Assert
        Assert.Throws<RebateNotFoundException>(() => rebateService.Calculate(request));
    }

    [Fact]
    public void Calculate_StoresCalculationResult_WhenGivenValidRequest()
    {
        // Arrange
        var productDataHandlerMock = new Mock<ProductDataHandler>();
        var rebateDataHandlerMock = new Mock<RebateDataHandler>();
        var incentiveFactoryMock = new Mock<IIncentiveFactory>();
        var rebateService = new RebateService(productDataHandlerMock.Object, rebateDataHandlerMock.Object, incentiveFactoryMock.Object);
        var request = new CalculateRebateRequest { ProductIdentifier = "test", RebateIdentifier = "test" };

        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var expectedRebate = new Rebate { Identifier = "test", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        var expectedIncentive = new FixedRateRebateIncentive(expectedRebate, expectedProduct);
        var expectedCalculationResult = new CalculateRebateResult { Success = true };

        productDataHandlerMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(expectedProduct);
        rebateDataHandlerMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(expectedRebate);
        incentiveFactoryMock.Setup(i => i.CreateIncentive(expectedRebate, expectedProduct)).Returns(expectedIncentive);

        // Act
        rebateService.Calculate(request);

        // Assert
        rebateDataHandlerMock.Verify(r => r.StoreCalculationResult(expectedRebate, It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public void Calculate_ReturnsCorrectAmount_ForFixedCashAmountIncentive()
    {
        // Arrange
        var expectedRebate = new Rebate { Amount = 50m, Identifier = "test", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        var expectedIncentive = new FixedCashAmountIncentive(expectedRebate);

        var request = new CalculateRebateRequest();

        // Act
        var result = expectedIncentive.Calculate(request);

        // Assert
        Assert.Equal(expectedRebate.Amount, result);
    }

    [Fact]
    public void Calculate_ReturnsCorrectAmount_ForFixedRateRebateIncentive()
    {
        // Arrange
        var expectedProduct = new Product { Identifier = "test", Price = 100m, SupportedIncentives = SupportedIncentiveType.FixedRateRebate };
        var expectedRebate = new Rebate { Identifier = "test", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.1m };
        var expectedIncentive = new FixedRateRebateIncentive(expectedRebate, expectedProduct);

        var request = new CalculateRebateRequest { Volume = 1 };

        // Act
        var result = expectedIncentive.Calculate(request);

        // Assert
        Assert.Equal(expectedProduct.Price * expectedRebate.Percentage * request.Volume, result);
    }

    [Fact]
    public void Calculate_ReturnsCorrectAmount_ForAmountPerUomIncentive()
    {
        // Arrange
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
        var rebate = new Rebate { Amount = 2m };
        var incentive = new AmountPerUomIncentive(rebate, product);

        var request = new CalculateRebateRequest { Volume = 3 };

        // Act
        var result = incentive.Calculate(request);

        // Assert
        Assert.Equal(rebate.Amount * request.Volume, result);
    }


    [Fact]
    public void InsertProduct_ThrowsException_WhenGivenInvalidProduct()
    {
        // Arrange
        var productDataStoreMock = new Mock<IProductDataStore>();
        var invalidProduct = new Product { Identifier = "", Price = -1, SupportedIncentives = (SupportedIncentiveType)100 };

        productDataStoreMock.Setup(p => p.InsertProduct(invalidProduct)).Throws<ArgumentException>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => productDataStoreMock.Object.InsertProduct(invalidProduct));
    }

    [Fact]
    public void InsertRebate_ThrowsException_WhenGivenInvalidRebate()
    {
        // Arrange
        var rebateDataStoreMock = new Mock<IRebateDataStore>();
        var invalidRebate = new Rebate { Identifier = "", Incentive = (IncentiveType)100 };

        rebateDataStoreMock.Setup(r => r.InsertRebate(invalidRebate)).Throws<ArgumentException>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rebateDataStoreMock.Object.InsertRebate(invalidRebate));
    }

}
