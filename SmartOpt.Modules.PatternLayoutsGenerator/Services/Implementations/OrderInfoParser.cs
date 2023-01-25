using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class OrderInfoParser : IOrderInfoParser
{
    public IList<OrderInfo> ParseOrdersFromActiveExcelWorksheet()
    {
        object? excelCom = Marshal.GetActiveObject("Excel.Application");
        if (excelCom == null)
        {
            throw new InvalidOperationException("Unable to connect with an active Excel application");
        }

        object? application = excelCom as Application;
        object? activeWorkbook = (application as Application)!.ActiveWorkbook;

        string workbookFilename = (activeWorkbook as Workbook)!.FullName;

        ReleaseObject(ref application);
        ReleaseObject(ref activeWorkbook);
        
        return ParseOrdersFromExcelWorksheetInternal(workbookFilename);
    }
    
    public IList<OrderInfo> ParseOrdersFromExcelWorksheet(string workbookFilepath)
    {
        return ParseOrdersFromExcelWorksheetInternal(workbookFilepath);
    }
    
    private IList<OrderInfo> ParseOrdersFromExcelWorksheetInternal(string workbookFilepath)
    {
        var tempFilepath = $"{Path.Combine(Path.GetDirectoryName(workbookFilepath), Path.GetFileNameWithoutExtension(workbookFilepath))}_temp{Path.GetExtension(workbookFilepath)}";
        
        try
        {
            File.Copy(workbookFilepath, tempFilepath, true);
        
            using var workbook = new XLWorkbook(tempFilepath);
            IXLWorksheet worksheet = workbook.Worksheets.First();
            
            IReadOnlyList<string> nameColumnValues = ParseColumn<string>(worksheet, 1, 3);
            IReadOnlyList<int> widthColumnValues = ParseColumn<int>(worksheet, 3, 3);
            // IReadOnlyList<int> requestKilosColumnValues = ParseColumn<int>(worksheet, 4, 3);
            // IReadOnlyList<double> doneKilosColumnValues = ParseColumn<double>(worksheet, 5, 3);
            // IReadOnlyList<int> unknownValuesColumnValues = ParseColumn<int>(worksheet, 13, 3);
            IReadOnlyList<double> countColumnValues = ParseColumn<double>(worksheet, 14, 3);

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
            var tempFile = new FileInfo(tempFilepath);
            if (tempFile.Exists)
            {
                tempFile.Delete();
            }
        }
    }
    
    /// <summary>
    /// Parse the values of the specified column
    /// </summary>
    /// <param name="worksheet">Excel worksheet</param>
    /// <param name="column">Column id</param>
    /// <param name="rowOffset">Row id offset</param>
    /// <typeparam name="T">Type of the column value</typeparam>
    /// <returns>The list of values of the parsed column </returns>
    /// <exception cref="InvalidOperationException">It's unable to read a value of the specified column</exception>
    private static IReadOnlyList<T> ParseColumn<T>(IXLWorksheet worksheet, int column, int rowOffset)
    {
        IXLRangeRows visibleRows = worksheet.RangeUsed().Rows(row => !row.WorksheetRow().IsHidden);

        var values = new List<T>();
        foreach (IXLRangeRow row in visibleRows.TakeLast(visibleRows.Count() - rowOffset))
        {
            IXLCell? cell = row.Cell(column);
            
            if (cell == null)
            {
                continue;
            }

            cell.TryGetValue(out T value);

            values.Add(value);
        }

        return values.AsIReadOnlyList();
    }
    
    protected static void ReleaseObject(ref object? obj)
    {
        if (obj != null && Marshal.IsComObject(obj))
        {
            Marshal.ReleaseComObject(obj);
        }

        obj = null;
    }
}