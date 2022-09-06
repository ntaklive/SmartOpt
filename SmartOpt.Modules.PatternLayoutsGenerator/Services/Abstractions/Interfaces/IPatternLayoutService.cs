using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IPatternLayoutService
{
    public IList<PatternLayout> CreatePatternLayoutsFromOrders(
        IList<OrderInfo> orderInfos, int maxWidth, double maxWaste, int groupSize);
}