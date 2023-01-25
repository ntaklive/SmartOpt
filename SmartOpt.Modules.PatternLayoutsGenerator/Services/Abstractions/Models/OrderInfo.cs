using System;
using SmartOpt.Core.Infrastructure.Interfaces;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class OrderInfo : ICloneable<OrderInfo>
{
    private string _name = string.Empty;
    private int _width;
    private double _rollsCount;

    public OrderInfo(string name, int width, double rollsCount)
    {
        Name = name;
        Width = width;
        RollsCount = rollsCount;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Order name cannot be empty");
            }

            _name = value;
        }
    }

    public int Width
    {
        get => _width;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentNullException(nameof(value), "Order width cannot be less than or equal to 0");
            }

            _width = value;
        }
    }

    public double RollsCount
    {
        get => _rollsCount;
        set
        {
            // todo: there are exceptions if count is 0 or negative
            // if (value <= 0)
            // {
            //     throw new ArgumentNullException(nameof(value), "The number of order rolls cannot be less than or equal to 0");
            // }

            _rollsCount = value;
        }
    }

    public OrderInfo Clone()
    {
        return new OrderInfo(Name, Width, RollsCount);
    }
    
    public override string ToString()
    {
        return " Width: " + Width + "; RollsCount: " + RollsCount;
    }
}
