using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutService : IPatternLayoutService
{
    private readonly IPatternLayoutGenerator _patternLayoutGenerator;
    private readonly IOrderInfoAggregator _orderInfoAggregator;
    private readonly IOrderInfoParser _orderInfoParser;

    public PatternLayoutService(
        IPatternLayoutGenerator patternLayoutGenerator,
        IOrderInfoAggregator orderInfoAggregator,
        IOrderInfoParser orderInfoParser)
    {
        _patternLayoutGenerator = patternLayoutGenerator;
        _orderInfoAggregator = orderInfoAggregator;
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
        IList<OrderInfo> mergedOrders = _orderInfoAggregator.AggregateOrdersWithIdenticalWidth(orders);
        Report report = _patternLayoutGenerator.GeneratePatternLayoutsFromOrders(
            mergedOrders.AsIReadOnlyCollection(), maxWidth, maxWaste, groupSize);

        return report;
    }

}