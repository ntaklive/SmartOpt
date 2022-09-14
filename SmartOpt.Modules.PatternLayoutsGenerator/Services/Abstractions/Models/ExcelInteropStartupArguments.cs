using Microsoft.Office.Interop.Excel;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class ExcelInteropStartupArguments
{
    public bool UseActiveWorksheet { get; set; }
    public bool IsVisible { get; set; }
    public bool DisplayAlerts { get; set; }
    public XlWindowState WindowState { get; set; }
    public string? WorkbookFilepath { get; set; }
}