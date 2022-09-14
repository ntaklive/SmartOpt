using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IExcelInteropWrapper
{
    public void WriteValueInCell(int x, int y, object value);
    public void MergeCells(int x1, int y1, int x2, int y2);
    public void SetVerticalAlignmentForRange(int x1, int y1, int x2, int y2, XlVAlign align);
    public void SetHorizontalAlignmentForRange(int x1, int y1, int x2, int y2, XlHAlign align);
    public void AutoFitColumnsInRange(int x1, int y1, int x2, int y2);
    public void ReleaseComObjects();
    public void StartExcelApplication(ExcelInteropStartupArguments startupArguments);

    /// <summary>
    /// Parse the values of the specified column
    /// </summary>
    /// <param name="column">Column id</param>
    /// <param name="rowOffset">Row offset</param>
    /// <typeparam name="T">Type of the column value</typeparam>
    /// <returns>The list of values of the parsed column </returns>
    /// <exception cref="InvalidOperationException">It's unable to read a value of the specified column</exception>
    public IReadOnlyList<T> ParseColumn<T>(int column, int rowOffset);
}