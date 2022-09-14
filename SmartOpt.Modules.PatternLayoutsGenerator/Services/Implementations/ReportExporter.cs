using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ClosedXML.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class ReportExporter : IReportExporter
{
    public void ExportToNewExcelWorkbook(Report report)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet worksheet = workbook.Worksheets.Add("Результаты");
        
        var startFromRow = 3;
        int index;
        var patternLayoutIndex = 1;

        worksheet.Cell(1, 1).SetValue("Обработано");
        worksheet.Range(1, 1, 1, 6).Merge();
        
        worksheet.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        worksheet.Columns().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        worksheet.Cell(2, 1).SetValue("№");
        worksheet.Cell(2, 2).SetValue("Наименования");
        worksheet.Cell(2, 3).SetValue("Ширина");
        worksheet.Cell(2, 4).SetValue("Сколько изготовить");
        worksheet.Cell(2, 5).SetValue("Отходы %");
        worksheet.Cell(2, 6).SetValue("Сколько раз");
        
        IReadOnlyList<PatternLayout> patternLayouts = report.GetPatternLayouts();
        foreach (PatternLayout patternLayout in patternLayouts)
        {
            index = 0;
            IReadOnlyList<OrderInfo> orders = patternLayout.Orders;
            foreach (OrderInfo order in orders)
            {
                index++;
                worksheet.Cell(startFromRow + index - 1, 2).SetValue(order.Name);
                worksheet.Cell(startFromRow + index - 1, 3).SetValue(order.Width);
                worksheet.Cell(startFromRow + index - 1, 4).SetValue((int) order.RollsCount);
            }
        
            worksheet.Range(startFromRow, 1, startFromRow + index - 1, 1).Merge();
            worksheet.Range(startFromRow, 5, startFromRow + index - 1, 5).Merge();
            worksheet.Range(startFromRow, 6, startFromRow + index - 1, 6).Merge();
            
            worksheet.Cell(startFromRow, 1).SetValue(patternLayoutIndex);
            worksheet.Cell(startFromRow, 5).SetValue(Math.Round(patternLayout.Waste, 2));
            worksheet.Cell(startFromRow, 6).SetValue(patternLayout.RollsCount);
        
            startFromRow += index;
            patternLayoutIndex++;
        }

        worksheet.Cell(startFromRow, 1).SetValue("Не обработано");
        worksheet.Range(startFromRow, 1, startFromRow, 6).Merge();

        startFromRow++;
        
        index = 0;
        IReadOnlyList<OrderInfo> ungroupedOrders = report.GetUngroupedOrders();
        foreach (OrderInfo order in ungroupedOrders)
        {
            index++;
            worksheet.Cell(startFromRow + index - 1, 2).SetValue(order.Name);
            worksheet.Cell(startFromRow + index - 1, 3).SetValue(order.Width);
            worksheet.Cell(startFromRow + index - 1, 4).SetValue((int) order.RollsCount);
        }
        
        worksheet.Range(startFromRow, 1, startFromRow + index - 1, 1).Merge();
        worksheet.Range(startFromRow, 5, startFromRow + index - 1, 5).Merge();
        worksheet.Range(startFromRow, 6, startFromRow + index - 1, 6).Merge();

        worksheet.Cell(startFromRow, 1).SetValue(patternLayoutIndex);

        worksheet.Columns().AdjustToContents();

        var newFilename = $"{Guid.NewGuid().ToString().Replace("-", "")}.xlsx";
        string newFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName)!,"Reports", newFilename);
        workbook.SaveAs(newFilepath);
        
        Process.Start(newFilepath);
    }
}