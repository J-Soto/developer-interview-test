using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore
{
    private static RebateDataStore instance;
    private SQLiteConnection connection;

    private RebateDataStore()
    {
        connection = new SQLiteConnection("Data Source=:memory:");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE Rebates (
                Identifier TEXT PRIMARY KEY,
                Incentive INTEGER,
                Amount DECIMAL(18, 2),
                Percentage DECIMAL(18, 2)
            );
        ";
        command.ExecuteNonQuery();
    }

    public static RebateDataStore Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RebateDataStore();
            }
            return instance;
        }
    }
    // En RebateDataStore.cs
    public List<Rebate> GetAllRebates()
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Rebates";

        var reader = command.ExecuteReader();
        var rebates = new List<Rebate>();

        while (reader.Read())
        {
            var rebate = new Rebate
            {
                Identifier = reader.GetString(0),
                Incentive = (IncentiveType)reader.GetInt32(1),
                Amount = reader.GetDecimal(2),
                Percentage = reader.GetDecimal(3)
            };
            rebates.Add(rebate);
        }

        return rebates;
    }
    public Rebate GetRebate(string rebateIdentifier)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT * FROM Rebates
            WHERE Identifier = @rebateIdentifier
        ";
        command.Parameters.AddWithValue("@rebateIdentifier", rebateIdentifier);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Rebate
            {
                Identifier = reader.GetString(0),
                Incentive = (IncentiveType)reader.GetInt32(1),
                Amount = reader.GetDecimal(2),
                Percentage = reader.GetDecimal(3)
            };
        }

        return null;
    }

    public void InsertTestRebate()
    {
        for (int i = 1; i <= 10; i++)
        {
            var command = connection.CreateCommand();
            command.CommandText = $@"
            INSERT INTO Rebates (Identifier, Incentive, Amount, Percentage)
            VALUES ('testRebate{i}', {i % 3}, {100.0 * i}, {10.0 * i})
        ";
            command.ExecuteNonQuery();
        }
    }

    public void InsertRebate(Rebate rebate)
    {
        if (rebate == null)
        {
            throw new ArgumentNullException(nameof(rebate));
        }

        if (string.IsNullOrWhiteSpace(rebate.Identifier))
        {
            throw new ArgumentException("Rebate identifier cannot be empty.");
        }

        if (!Enum.IsDefined(typeof(IncentiveType), rebate.Incentive))
        {
            throw new ArgumentException("Incentive type is not valid.");
        }
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Rebates (Identifier, Incentive, Amount, Percentage)
            VALUES (@identifier, @incentive, @amount, @percentage)
        ";
        command.Parameters.AddWithValue("@identifier", rebate.Identifier);
        command.Parameters.AddWithValue("@incentive", (int)rebate.Incentive);
        command.Parameters.AddWithValue("@amount", rebate.Amount);
        command.Parameters.AddWithValue("@percentage", rebate.Percentage);

        command.ExecuteNonQuery();
    }

    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Rebates
            SET Amount = @rebateAmount
            WHERE Identifier = @rebateIdentifier
        ";
        command.Parameters.AddWithValue("@rebateAmount", rebateAmount);
        command.Parameters.AddWithValue("@rebateIdentifier", rebate.Identifier);

        command.ExecuteNonQuery();
    }
}