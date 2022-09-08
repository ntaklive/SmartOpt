using System.Collections.Generic;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class PatternLayout
{
    public PatternLayout(IEnumerable<OrderInfo> orders, double rollsCount, double waste)
    {
        Waste = waste;
        Orders = orders.AsIReadOnlyCollection();
        RollsCount = rollsCount;
    }

    public IReadOnlyCollection<OrderInfo> Orders { get; }
    
    public double RollsCount { get; }
    
    public double Waste { get; }

    public override string ToString()
    {
        return $"Waste: {Waste}; RollsCount: {RollsCount}";
    }
}