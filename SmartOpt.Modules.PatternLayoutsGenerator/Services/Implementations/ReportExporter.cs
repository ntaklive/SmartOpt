using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class ReportExporter : IReportExporter
{
    private readonly IExcelInteropWrapper _excel;

    public ReportExporter(IExcelInteropWrapper excel)
    {
        _excel = excel;
    }
    
    public void ExportToNewExcelWorkbook(Report report)
    {
        _excel.StartExcelApplication(new ExcelInteropStartupArguments
        {
            IsVisible = true,
            DisplayAlerts = false,
            WindowState = XlWindowState.xlMaximized,
            UseActiveWorksheet = false,
            WorkbookFilepath = null
        });
        
        var startFromRow = 3;
        int index;
        var patternLayoutIndex = 1;

        _excel.WriteValueInCell(1, 1, "Обработано");
        _excel.MergeCells(1, 1, 1, 6);

        _excel.SetVerticalAlignmentForRange(1, 1, 6, 1, XlVAlign.xlVAlignCenter);
        _excel.SetHorizontalAlignmentForRange(1, 1, 6, 1, XlHAlign.xlHAlignCenter);

        _excel.WriteValueInCell(2, 1,  "№");
        _excel.WriteValueInCell(2, 2,  "Наименования");
        _excel.WriteValueInCell(2, 3,  "Ширина");
        _excel.WriteValueInCell(2, 4,  "Сколько изготовить");
        _excel.WriteValueInCell(2, 5,  "Отходы %");
        _excel.WriteValueInCell(2, 6,  "Сколько раз");
        
        IReadOnlyList<PatternLayout> patternLayouts = report.GetPatternLayouts();
        foreach (PatternLayout patternLayout in patternLayouts)
        {
            index = 0;
            IReadOnlyList<OrderInfo> orders = patternLayout.Orders;
            foreach (OrderInfo order in orders)
            {
                index++;
                _excel.WriteValueInCell(startFromRow + index - 1, 2, order.Name);
                _excel.WriteValueInCell(startFromRow + index - 1, 3, order.Width);
                _excel.WriteValueInCell(startFromRow + index - 1, 4, (int) order.RollsCount);
            }
        
            _excel.MergeCells(startFromRow, 1, startFromRow + index - 1, 1);
            _excel.MergeCells(startFromRow, 5, startFromRow + index - 1, 5);
            _excel.MergeCells(startFromRow, 6, startFromRow + index - 1, 6);
            
            _excel.WriteValueInCell(startFromRow, 1, patternLayoutIndex);
            _excel.WriteValueInCell(startFromRow, 5, patternLayout.Waste);
            _excel.WriteValueInCell(startFromRow, 6, patternLayout.RollsCount);
        
            startFromRow += index;
            patternLayoutIndex++;
        }
        
        _excel.MergeCells(startFromRow, 1, startFromRow, 6);
        _excel.WriteValueInCell(startFromRow, 1, "Не обработано");
        
        startFromRow++;
        
        index = 0;
        IReadOnlyList<OrderInfo> ungroupedOrders = report.GetUngroupedOrders();
        foreach (OrderInfo order in ungroupedOrders)
        {
            index++;
            _excel.WriteValueInCell(startFromRow + index - 1, 2, order.Name);
            _excel.WriteValueInCell(startFromRow + index - 1, 3, order.Width);
            _excel.WriteValueInCell(startFromRow + index - 1, 4, (int) order.RollsCount);
        }
        
        _excel.MergeCells(startFromRow, 1, startFromRow + index - 1, 1);
        _excel.MergeCells(startFromRow, 5, startFromRow + index - 1, 5);
        _excel.MergeCells(startFromRow, 6, startFromRow + index - 1, 6);

        _excel.WriteValueInCell(startFromRow, 1, patternLayoutIndex);

        _excel.AutoFitColumnsInRange(2, 2, startFromRow, 6);

        _excel.ReleaseComObjects();
    }
}