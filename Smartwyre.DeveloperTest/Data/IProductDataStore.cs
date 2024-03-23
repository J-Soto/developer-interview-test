using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

public interface IProductDataStore
{
    Product GetProduct(string productIdentifier);
    void InsertProduct(Product product);
    void InsertTestProduct();
    List<Product> GetAllProducts();

}