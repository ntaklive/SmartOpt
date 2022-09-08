using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class ReportExporter : IReportExporter
{
    public void ExportToNewExcelWorkbook(Report report)
    {
        ICollection<PatternLayout> patternLayouts = report.PatternLayouts;

        List<PatternLayout> data = patternLayouts.Take(patternLayouts.Count() - 1).ToList();
        PatternLayout remnants = patternLayouts.Last();
        var app = new Application
        {
            Visible = true,
            DisplayAlerts = false,
            WindowState = XlWindowState.xlMaximized
        };
        app.Workbooks.Add();
        var activeWorksheet = (app.ActiveSheet as Worksheet)!;
        activeWorksheet.Name = "Results";

        var startFromRow = 3;
        var index = 0;
        var orderIndex = 1;

        activeWorksheet.Range[activeWorksheet.Cells[1, 1], activeWorksheet.Cells[1, 6]].Merge();
        activeWorksheet.Cells[1, 1] = "Обработано";

        ((Style) activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 6], activeWorksheet.Cells[startFromRow, 7]]
                .Style)
            .VerticalAlignment = XlVAlign.xlVAlignCenter;
        ((Style) activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 6], activeWorksheet.Cells[startFromRow, 7]]
                .Style)
            .HorizontalAlignment =
            XlHAlign.xlHAlignCenter;

        activeWorksheet.Cells[2, 1] = "№";
        activeWorksheet.Cells[2, 2] = "Наименования";
        activeWorksheet.Cells[2, 3] = "Ширина";
        activeWorksheet.Cells[2, 4] = "Сколько изготовить";
        activeWorksheet.Cells[2, 5] = "Отходы %";
        activeWorksheet.Cells[2, 6] = "Сколько раз";
        foreach (PatternLayout? order in data)
        {
            index = 0;
            List<OrderInfo> o = order.Orders;
            foreach (OrderInfo orderInfo in o)
            {
                index++;
                activeWorksheet.Cells[startFromRow + index - 1, 2] = orderInfo.Name;
                activeWorksheet.Cells[startFromRow + index - 1, 3] = orderInfo.Width;
                activeWorksheet.Cells[startFromRow + index - 1, 4] = (int) orderInfo.RollsCount;
            }

            activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 1],
                activeWorksheet.Cells[startFromRow + index - 1, 1]].Merge();
            activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 5],
                activeWorksheet.Cells[startFromRow + index - 1, 5]].Merge();
            activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 6],
                activeWorksheet.Cells[startFromRow + index - 1, 6]].Merge();
            activeWorksheet.Cells[startFromRow, 1] = orderIndex;
            activeWorksheet.Cells[startFromRow, 5] = Math.Round(order.Waste, 2);
            activeWorksheet.Cells[startFromRow, 6] = order.RollsCount;
            startFromRow += index;
            orderIndex++;
        }

        activeWorksheet.Range[activeWorksheet.Cells[startFromRow, 1], activeWorksheet.Cells[startFromRow, 6]].Merge();
        activeWorksheet.Cells[startFromRow, 1] = "Не обработано";
        startFromRow++;

        index = 0;
        List<OrderInfo> o2 = remnants.Orders;
        foreach (OrderInfo orderInfo in o2)
        {
            index++;
            activeWorksheet.Cells[startFromRow + index - 1, 2] = orderInfo.Name;
            activeWorksheet.Cells[startFromRow + index - 1, 3] = orderInfo.Width;
            activeWorksheet.Cells[startFromRow + index - 1, 4] = orderInfo.RollsCount;
        }

        activeWorksheet
            .Range[activeWorksheet.Cells[startFromRow, 1], activeWorksheet.Cells[startFromRow + index - 1, 1]].Merge();
        activeWorksheet
            .Range[activeWorksheet.Cells[startFromRow, 5], activeWorksheet.Cells[startFromRow + index - 1, 5]].Merge();
        activeWorksheet
            .Range[activeWorksheet.Cells[startFromRow, 6], activeWorksheet.Cells[startFromRow + index - 1, 6]].Merge();
        activeWorksheet.Cells[startFromRow, 1] = orderIndex;
        activeWorksheet.Range[activeWorksheet.Cells[2, 2], activeWorksheet.Cells[startFromRow, 6]].Columns.AutoFit();

        // todo: Not working
        // if (isRectanglesDraw)
        // {
        //     DrawResults(activeWorksheet, data);
        // }

        Marshal.ReleaseComObject(app);
        Marshal.ReleaseComObject(activeWorksheet);
    }
}