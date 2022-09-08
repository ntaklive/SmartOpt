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
    public IList<OrderInfo> ParseOrdersFromActiveExcelWorksheet()
    {
        return ParseOrdersFromExcelWorksheetInternal(GetExcel());
    }
    
    public IList<OrderInfo> ParseOrdersFromExcelWorksheet(string workbookFilepath)
    {
        return ParseOrdersFromExcelWorksheetInternal(GetExcel(workbookFilepath));
    }
    
    private IList<OrderInfo> ParseOrdersFromExcelWorksheetInternal(Application excel)
    {
        try
        {
            if (excel.ActiveSheet is not Worksheet activeSheet)
            {
                throw new InvalidOperationException("Unable to find any active worksheet");
            }

            List<string> nameColumnValues = ParseColumn<string>(activeSheet, 1);
            List<int> widthColumnValues = ParseColumn<int>(activeSheet, 3);
            List<double> countColumnValues = ParseColumn<double>(activeSheet, 14);

            OrderInfo[] orders = nameColumnValues.Select((name, i) =>
                new OrderInfo(name, widthColumnValues[i], countColumnValues[i])).ToArray();

            return orders;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("An unexpected error was occured", exception);
        }
        finally
        {
            if (excel.ActiveSheet != null)
            {
                Marshal.ReleaseComObject(excel.ActiveSheet);
            }

            Marshal.ReleaseComObject(excel);
        }
    }

    private Application GetExcel(string? workbookFilepath = null)
    {
        Application excel;
        if (workbookFilepath == null)
        {
            object? excelCom = Marshal.GetActiveObject("Excel.Application");
            if (excelCom == null)
            {
                throw new InvalidOperationException("Unable to connect with an active Excel application");
            }
            
            excel = (excelCom as Application)!;
        }
        else
        {
            excel = new Application();
            excel.Workbooks.Add(workbookFilepath);
        }

        return excel;
    }

    /// <summary>
    /// Parse the values of the specified column
    /// </summary>
    /// <param name="worksheet">Excel worksheet</param>
    /// <param name="column">Column id</param>
    /// <typeparam name="T">Type of the column value</typeparam>
    /// <returns>The list of values of the parsed column </returns>
    /// <exception cref="InvalidOperationException">It's unable to read a value of the specified column</exception>
    private List<T> ParseColumn<T>(Worksheet worksheet, int column)
    {
        dynamic? range = worksheet.UsedRange.Columns[column];
        range = range.Resize[range.Rows.Count].Offset[3, 0];
        range = range.SpecialCells(XlCellType.xlCellTypeVisible, Type.Missing);

        var values = new List<T>();
        foreach (Range area in range.Areas)
        {
            foreach (Range row in area.Rows)
            {
                if (row.Value == null)
                {
                    continue;
                }

                T value = Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Int32 => int.Parse(row.Value.ToString()),
                    TypeCode.String => row.Value.ToString(),
                    TypeCode.Decimal => decimal.Parse(row.Value.ToString()),
                    TypeCode.Double => double.Parse(row.Value.ToString()),
                    _ => throw new InvalidOperationException("Unable to read the value of the specified column")
                };

                values.Add(value);
            }
        }

        return values;
    }
}