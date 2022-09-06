using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IOrderInfoMerger
{
    public IList<OrderInfo> MergeOrdersWithIdenticalWidth(ICollection<OrderInfo> orders);
}