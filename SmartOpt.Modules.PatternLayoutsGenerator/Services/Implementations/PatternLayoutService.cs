using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutService : IPatternLayoutService
{
    private readonly IPatternLayoutGenerator _patternLayoutGenerator;
    private readonly IOrderInfoMerger _orderInfoMerger;
    private readonly IOrderInfoParser _orderInfoParser;

    public PatternLayoutService(
        IPatternLayoutGenerator patternLayoutGenerator,
        IOrderInfoMerger orderInfoMerger,
        IOrderInfoParser orderInfoParser)
    {
        _patternLayoutGenerator = patternLayoutGenerator;
        _orderInfoMerger = orderInfoMerger;
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
        IList<OrderInfo> mergedOrders = _orderInfoMerger.MergeOrdersWithIdenticalWidth(orders);
        IList<PatternLayout> patternLayouts = _patternLayoutGenerator.GeneratePatternLayoutsFromOrders(
            mergedOrders, maxWidth, maxWaste, groupSize);

        return new Report(patternLayouts);
    }

}