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
        Report report = _patternLayoutGenerator.GeneratePatternLayoutsFromOrders(
            orders.AsIReadOnlyList(), maxWidth, maxWaste, groupSize);

        return report;
    }

    
}