using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IOrderInfoAggregator
{
    public IList<OrderInfo> AggregateOrdersWithIdenticalWidth(ICollection<OrderInfo> orders);
}