using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class OrderInfoParser : IOrderInfoParser
{
    private readonly IExcelInteropWrapper _excel;

    public OrderInfoParser(IExcelInteropWrapper excel)
    {
        _excel = excel;
    }

    public IList<OrderInfo> ParseOrdersFromActiveExcelWorksheet()
    {
        return ParseOrdersFromExcelWorksheetInternal(true, null);
    }

    public IList<OrderInfo> ParseOrdersFromExcelWorksheet(string workbookFilepath)
    {
        return ParseOrdersFromExcelWorksheetInternal(false, workbookFilepath);
    }

    private IList<OrderInfo> ParseOrdersFromExcelWorksheetInternal(bool useActiveWorksheet, string? workbookFilepath)
    {
        _excel.StartExcelApplication(new ExcelInteropStartupArguments
        {
            IsVisible = false,
            DisplayAlerts = false,
            WindowState = XlWindowState.xlNormal,
            UseActiveWorksheet = useActiveWorksheet,
            WorkbookFilepath = workbookFilepath
        });

        try
        {
            IReadOnlyList<string> nameColumnValues = _excel.ParseColumn<string>(1, 3);
            IReadOnlyList<int> widthColumnValues = _excel.ParseColumn<int>(3, 3);
            IReadOnlyList<double> countColumnValues = _excel.ParseColumn<double>(14, 3);

            OrderInfo[] orders = nameColumnValues.Select((name, i) =>
                new OrderInfo(name, widthColumnValues[i], countColumnValues[i])).ToArray();

            return orders;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("An unexpected error was occured while parsing", exception);
        }
        finally
        {
            _excel.ReleaseComObjects();
        }
    }
}