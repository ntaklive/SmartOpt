using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Theraot.Collections;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class ExcelInteropWrapper : ExcelInteropWrapperBase
{
    private readonly List<object?> _comObjectsList = new();

    private Application _application;
    private Workbooks _workbooks;
    private Workbook _workbook;
    private Worksheet _activeWorksheet;
    
    public override void WriteValueInCell(int x, int y, object value)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);
        
        cells[x, y] = value;
    }

    public override void MergeCells(int x1, int y1, int x2, int y2)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);

        Range range = _activeWorksheet.Range[cells[x1, y1], cells[x2, y2]];
        _comObjectsList.Add(range);
        
        range.Merge();
    }

    public override void SetVerticalAlignmentForRange(int x1, int y1, int x2, int y2, XlVAlign align)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);
        
        Range range = _activeWorksheet.Range[cells[x1, y1], cells[x2, y2]];
        _comObjectsList.Add(range);

        Style style = range.Style;
        _comObjectsList.Add(style);

        style.VerticalAlignment = align;
    }

    public override void SetHorizontalAlignmentForRange(int x1, int y1, int x2, int y2, XlHAlign align)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);
        
        Range range = _activeWorksheet.Range[cells[x1, y1], cells[x2, y2]];
        _comObjectsList.Add(range);

        Style style = range.Style;
        _comObjectsList.Add(style);

        style.HorizontalAlignment = align;
    }

    public override void AutoFitColumnsInRange(int x1, int y1, int x2, int y2)
    {
        Range cells = _activeWorksheet.Cells;
        _comObjectsList.Add(cells);

        Range range = _activeWorksheet.Range[cells[x1, y1], cells[x2, y2]];
        _comObjectsList.Add(range);

        Range columns = range.Columns;
        _comObjectsList.Add(columns);

        columns.AutoFit();
    }

    public override void ReleaseComObjects()
    {
        for (int i = _comObjectsList.Count - 1; i >= 0; i--)
        {
            object? comObject = _comObjectsList[i]; 
            ReleaseObject(ref comObject);
        }
    }

    public override void StartExcelApplication(ExcelInteropStartupArguments startupArguments)
    {
        if (startupArguments.UseActiveWorksheet)
        {
            object? excelCom = Marshal.GetActiveObject("Excel.Application");
            if (excelCom == null)
            {
                throw new InvalidOperationException("Unable to connect with an active Excel application");
            }

            _application = (excelCom as Application)!;
        }

        _application = new Application
        {
            Visible = startupArguments.IsVisible,
            DisplayAlerts = startupArguments.DisplayAlerts,
            WindowState = startupArguments.WindowState
        };
        
        _workbooks = _application.Workbooks;
        _workbook = string.IsNullOrWhiteSpace(startupArguments.WorkbookFilepath)
            ? _workbooks.Add()
            : _workbooks.Add(startupArguments.WorkbookFilepath);
        
        _activeWorksheet = _application.ActiveSheet;
        if (_activeWorksheet is not Worksheet)
        {
            throw new InvalidOperationException("Unable to find any active worksheet");
        }

        _comObjectsList.Add(_application);
        _comObjectsList.Add(_workbooks);
        _comObjectsList.Add(_workbook);
        _comObjectsList.Add(_activeWorksheet);
    }
    
    public override IReadOnlyList<T> ParseColumn<T>(int column, int rowOffset)
    {
        Range usedRange = _activeWorksheet.UsedRange;
        _comObjectsList.Add(usedRange);
        
        Range usedRangeColumns = usedRange.Columns;
        _comObjectsList.Add(usedRangeColumns);

        Range range = usedRangeColumns[column];
        _comObjectsList.Add(range);

        Range rangeResize = range.Resize;
        _comObjectsList.Add(rangeResize);

        Range rangeRows = range.Rows;
        _comObjectsList.Add(rangeRows);

        Range resizedRange = rangeResize[rangeRows.Count];
        _comObjectsList.Add(resizedRange);

        Range rangeOffset = resizedRange.Offset;
        _comObjectsList.Add(rangeOffset);

        Range offsetRange = rangeOffset[rowOffset, 1];
        _comObjectsList.Add(offsetRange);

        Range visibleColumnRange = offsetRange.SpecialCells(XlCellType.xlCellTypeVisible, Type.Missing);
        _comObjectsList.Add(visibleColumnRange);

        Areas visibleColumnRangeAreas = visibleColumnRange.Areas;
        _comObjectsList.Add(visibleColumnRangeAreas);

        var values = new List<T>();
        foreach (Range area in visibleColumnRangeAreas)
        {
            _comObjectsList.Add(area);
            
            Range areaRows = area.Rows;
            _comObjectsList.Add(areaRows);
            foreach (Range row in areaRows)
            {
                _comObjectsList.Add(row);
                
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

        return values.AsIReadOnlyList();
    }
}