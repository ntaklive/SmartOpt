using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IPatternLayoutGenerator
{
    public IList<PatternLayout> GeneratePatternLayoutsFromOrders(
        IList<OrderInfo> orderInfos, int maxWidth, double maxWaste, int groupSize);
}