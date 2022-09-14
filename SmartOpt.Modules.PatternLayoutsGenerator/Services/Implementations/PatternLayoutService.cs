using System.Collections.Generic;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutService : IPatternLayoutService
{
    private readonly IPatternLayoutGenerator _patternLayoutGenerator;
    private readonly IOrderInfoParser _orderInfoParser;

    public PatternLayoutService(
        IPatternLayoutGenerator patternLayoutGenerator,
        IOrderInfoParser orderInfoParser)
    {
        _patternLayoutGenerator = patternLayoutGenerator;
        _orderInfoParser = orderInfoParser;
    }
    
    public Report GeneratePatternLayoutsFromActiveExcelWorksheet(int maxWidth, double maxWaste, int groupSize)
    {
        IList<OrderInfo> orders = _orderInfoParser.ParseOrdersFromActiveExcelWorksheet();
        return GeneratePatternLayoutsFromExcelWorksheetInternal(orders, maxWidth, maxWaste, groupSize);
    }
    
    public Report GeneratePatternLayoutsFromExcelWorksheet(string workbookFilepath, int maxWidth, double maxWaste, int groupSize)
    {
        IList<OrderInfo> orders = _orderInfoParser.ParseOrdersFromExcelWorksheet(workbookFilepath);
        return GeneratePatternLayoutsFromExcelWorksheetInternal(orders, maxWidth, maxWaste, groupSize);
    }

    private Report GeneratePatternLayoutsFromExcelWorksheetInternal(
        IList<OrderInfo> orders, int maxWidth, double maxWaste, int groupSize)
    {
        IReadOnlyList<OrderInfo> mergedOrders = AggregateOrdersWithIdenticalWidth(orders);
        Report report = _patternLayoutGenerator.GeneratePatternLayoutsFromOrders(
            mergedOrders, maxWidth, maxWaste, groupSize);

        return report;
    }

    private static IReadOnlyList<OrderInfo> AggregateOrdersWithIdenticalWidth(IList<OrderInfo> orders)
    {
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
            .ToList()
            .AsIReadOnlyList();
    }
}