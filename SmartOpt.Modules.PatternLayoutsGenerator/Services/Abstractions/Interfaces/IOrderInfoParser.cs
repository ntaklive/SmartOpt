using System.Collections.Generic;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IOrderInfoParser
{
    public IList<OrderInfo> ParseOrdersFromActiveExcelWorksheet();
    
    // todo add worksheetId parameter. by default this method uses 0
    public IList<OrderInfo> ParseOrdersFromExcelWorksheet(string workbookFilepath);
}