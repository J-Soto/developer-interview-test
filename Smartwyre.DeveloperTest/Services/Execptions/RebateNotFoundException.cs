using System;

public class RebateNotFoundException : Exception
{
    public RebateNotFoundException(string message) : base(message) { }
}