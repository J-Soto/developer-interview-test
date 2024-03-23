using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore
{
    private static SQLiteConnection connection;
    private static readonly ProductDataStore instance = new ProductDataStore();

    private ProductDataStore()
    {
        connection = new SQLiteConnection("Data Source=:memory:");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE Product (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Identifier TEXT,
                Price DECIMAL,
                Uom TEXT,
                SupportedIncentives INTEGER
            );
        ";
        command.ExecuteNonQuery();
    }

    public static ProductDataStore Instance
    {
        get
        {
            return instance;
        }
    }

    public Product GetProduct(string productIdentifier)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Product WHERE Identifier = @identifier";
        command.Parameters.AddWithValue("@identifier", productIdentifier);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Identifier = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Uom = reader.GetString(3),
                SupportedIncentives = (SupportedIncentiveType)reader.GetInt32(4)
            };
        }
        else
        {
            return null;
        }
    }

    public void InsertTestProduct()
    {
        for (int i = 1; i <= 10; i++)
        {
            var command = connection.CreateCommand();
            command.CommandText = $@"
            INSERT INTO Product (Identifier, Price, Uom, SupportedIncentives)
            VALUES ('testProduct{i}', {100.0 * i}, 'unit', {1 << (i % 3)})
        ";
            command.ExecuteNonQuery();
        }
    }

    public void InsertProduct(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }
        if (string.IsNullOrWhiteSpace(product.Identifier))
        {
            throw new ArgumentException("Product identifier cannot be empty.");
        }
        if (product.Price <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero.");
        }
        if (!Enum.IsDefined(typeof(SupportedIncentiveType), product.SupportedIncentives))
        {
            throw new ArgumentException("Supported incentive type is not valid.");
        }

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Product (Identifier, Price, Uom, SupportedIncentives)
            VALUES (@identifier, @price, @uom, @supportedIncentives)
        ";
        command.Parameters.AddWithValue("@identifier", product.Identifier);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@uom", product.Uom);
        command.Parameters.AddWithValue("@supportedIncentives", (int)product.SupportedIncentives);

        command.ExecuteNonQuery();
    }

    public List<Product> GetAllProducts()
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Product";

        var products = new List<Product>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Identifier = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Uom = reader.GetString(3),
                SupportedIncentives = (SupportedIncentiveType)reader.GetInt32(4)
            });
        }

        return products;
    }








}
