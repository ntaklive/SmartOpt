using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IOrderInfoParser
{
    public IList<OrderInfo> ParseOrdersFromActiveExcelWorksheet();
    
    public IList<OrderInfo> ParseOrdersFromExcelWorksheet(string filePath);
}