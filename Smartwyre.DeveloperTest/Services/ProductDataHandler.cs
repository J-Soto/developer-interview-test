using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

public class ProductDataHandler
{
    private IProductDataStore _productDataStore;
    public ProductDataHandler()
    {
    }
    public ProductDataHandler(IProductDataStore productDataStore)
    {
        _productDataStore = productDataStore;
    }

    public virtual Product GetProduct(string productIdentifier)
    {
        return _productDataStore.GetProduct(productIdentifier);
    }
    public void InsertProduct(Product product)
    {
        _productDataStore.InsertProduct(product);
    }
    public void InsertTestProduct()
    {
        _productDataStore.InsertTestProduct();
    }
    public List<Product> GetAllProducts()
    {
        return _productDataStore.GetAllProducts();
    }
}