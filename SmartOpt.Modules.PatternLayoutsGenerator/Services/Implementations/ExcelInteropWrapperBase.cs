using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public abstract class ExcelInteropWrapperBase : IExcelInteropWrapper
{
    protected static void ReleaseObject(ref object? obj)
    {
        // Do not catch an exception from this.
        // You may want to remove these guards depending on
        // what you think the semantics should be.
        if (obj != null && Marshal.IsComObject(obj))
        {
            Marshal.ReleaseComObject(obj);
        }

        // Since passed "by ref" this assignment will be useful
        // (It was not useful in the original, and neither was the
        //  GC.Collect.)
        obj = null;
    }

    public abstract void WriteValueInCell(int x, int y, object value);
    public abstract void MergeCells(int x1, int y1, int x2, int y2);
    public abstract void SetVerticalAlignmentForRange(int x1, int y1, int x2, int y2, XlVAlign align);
    public abstract void SetHorizontalAlignmentForRange(int x1, int y1, int x2, int y2, XlHAlign align);
    public abstract void AutoFitColumnsInRange(int x1, int y1, int x2, int y2);
    public abstract void ReleaseComObjects();
    public abstract void StartExcelApplication(ExcelInteropStartupArguments startupArguments);
    public abstract IReadOnlyList<T> ParseColumn<T>(int column, int rowOffset);
}