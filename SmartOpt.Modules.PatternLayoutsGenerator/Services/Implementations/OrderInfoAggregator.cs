using System.Collections.Generic;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services
{
    public class OrderInfoAggregator : IOrderInfoAggregator
    {
        public IList<OrderInfo> AggregateOrdersWithIdenticalWidth(ICollection<OrderInfo> orders)
        {
            // todo перенести в алгоритм
            IEnumerable<int> elementsCount = orders
                .Select(x => x.Width)
                .Distinct();

            return elementsCount
                .Select(item => orders.Where(x => x.Width == item))
                .Select(tmp => tmp.Aggregate((prev, next) =>
                {
                    prev.Name += ", " + next.Name;
                    prev.RollsCount += next.RollsCount;
                    return prev;
                }))
                .ToList();
        }
    }
}