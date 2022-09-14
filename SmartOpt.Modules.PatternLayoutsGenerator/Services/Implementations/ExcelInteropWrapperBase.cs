using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public abstract class ExcelInteropWrapperBase : IExcelInteropWrapper
{
    protected void ReleaseObject(ref object? obj)
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

    public abstract void WriteTextInCell(int x, int y, string text);
    public abstract void MergeCells(int x1, int y1, int x2, int y2);
    public abstract void ReleaseComObjects();
}