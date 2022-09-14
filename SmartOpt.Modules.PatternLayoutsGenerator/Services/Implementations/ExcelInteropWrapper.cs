using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class ExcelInteropWrapper : ExcelInteropWrapperBase
{
    private readonly List<object?> _comObjectsList = new();

    private readonly Application _application;
    private readonly Workbooks _workbooks;
    private readonly Workbook _workbook;
    private readonly Worksheet _activeWorksheet;
    
    public ExcelInteropWrapper()
    {
        _application = new Application
        {
            Visible = true,
            DisplayAlerts = false,
            WindowState = XlWindowState.xlMaximized
        };

        _workbooks = _application.Workbooks;
        _workbook = _workbooks.Add();
        _activeWorksheet = _application.ActiveSheet;
        
        _comObjectsList.Add(_application);
        _comObjectsList.Add(_workbooks);
        _comObjectsList.Add(_workbook);
        _comObjectsList.Add(_activeWorksheet);
    }
    
    public override void WriteTextInCell(int x, int y, string text)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);
        
        cells[x, y] = text;
    }

    public override void MergeCells(int x1, int y1, int x2, int y2)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);

        Range range = _activeWorksheet.Range[cells[x1, y1], cells[x2, y2]];
        _comObjectsList.Add(range);
        
        range.Merge();
    }

    public override void ReleaseComObjects()
    {
        for (int i = _comObjectsList.Count - 1; i >= 0; i--)
        {
            object? comObject = _comObjectsList[i]; 
            ReleaseObject(ref comObject);
        }
    }
}