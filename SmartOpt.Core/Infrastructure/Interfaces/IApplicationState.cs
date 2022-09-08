using System;
using SmartOpt.Core.Infrastructure.Enums;

namespace SmartOpt.Core.Infrastructure.Interfaces;

public interface IApplicationState
{
    public string? ExcelBookFilepath { get; }
    public int? MaxWidth { get; }
    public double? MaxWaste { get; }
    public int? GroupSize { get; }
    OperationType OperationType  { get; }
    GuiType GuiType  { get; }
    
    public event Action<object, string>? ExcelBookFilepathChanged;
    public event Action<object, int?>? MaxWidthChanged;
    public event Action<object, double?>? MaxWasteChanged;
    public event Action<object, int?>? GroupSizeChanged;
    public event Action<object, OperationType>? OperationTypeChanged;
    public event Action<object, GuiType>? GuiTypeChanged;

    public void SetExcelWorkbookFilepath(object sender, string filepath);
    public void SetMaxWidth(object sender, int? maxWidth);
    public void SetMaxWaste(object sender, double? maxWaste);
    public void SetGroupSize(object sender, int? groupSize);
    public void SetOperationType(object sender, OperationType operationType);
    public void SetGuiType(object sender, GuiType guiType);
}