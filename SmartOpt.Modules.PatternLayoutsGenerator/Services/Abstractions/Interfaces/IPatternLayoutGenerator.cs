using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IPatternLayoutGenerator
{
    public Report GeneratePatternLayoutsFromOrders(IReadOnlyList<OrderInfo> orders,
        int maxWidth, double maxWaste, int groupSize);
}