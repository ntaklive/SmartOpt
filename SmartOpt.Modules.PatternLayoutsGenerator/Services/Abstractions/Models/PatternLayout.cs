using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class PatternLayout
{
    public PatternLayout(List<OrderInfo> orders, double waste)
    {
        Waste = waste;
        RollsCount = orders.Select(item => item.RollsCount).Min();
        orders.ForEach(order =>
        {
            Orders.Add(order.Clone());
            order.RollsCount -= RollsCount;
        });

        // todo заменить логгером либо убрать
        Array.ForEach(Orders.ToArray(), Console.WriteLine);
        Console.WriteLine();
        Console.WriteLine("Waste:" + waste);
        Console.WriteLine("Roll quantity: " + RollsCount);
        Console.WriteLine("-------------");
    }

    public List<OrderInfo> Orders { get; set; } = new List<OrderInfo>();
    
    public double RollsCount { get; set; }
    
    public double Waste { get; set; }
}