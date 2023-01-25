using System;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;

namespace SmartOpt.Core.Infrastructure;

public sealed class ApplicationState : IApplicationState
{
    public static readonly ApplicationState Default = new()
    {
        ExcelBookFilepath = string.Empty,
        GroupSize = 5,
        MaxWaste = 3.0,
        MaxWidth = 6000,
        OperationType = OperationType.None,
        GuiType = GuiType.Gui
    };

    public string? ExcelBookFilepath { get; private set; }
    public int? MaxWidth { get; private set; }
    public double? MaxWaste { get; private set; }
    public int? GroupSize { get; private set; }
    public OperationType OperationType { get; private set; }
    public GuiType GuiType { get; private set; }

    public event Action<object, string>? ExcelBookFilepathChanged;
    public event Action<object, int?>? MaxWidthChanged;
    public event Action<object, double?>? MaxWasteChanged;
    public event Action<object, int?>? GroupSizeChanged;
    public event Action<object, OperationType>? OperationTypeChanged;
    public event Action<object, GuiType>? GuiTypeChanged;

    public void SetExcelWorkbookFilepath(object sender, string filepath)
    {
        ExcelBookFilepath = filepath;
        OnExcelBookFilepathChanged(sender, ExcelBookFilepath);
    }

    public void SetMaxWidth(object sender, int? maxWidth)
    {
        MaxWidth = maxWidth;
        OnMaxWidthChanged(sender, MaxWidth);
    }

    public void SetMaxWaste(object sender, double? maxWaste)
    {
        MaxWaste = maxWaste;
        OnMaxWasteChanged(sender, MaxWaste);
    }

    public void SetGroupSize(object sender, int? groupSize)
    {
        GroupSize = groupSize;
        OnGroupSizeChanged(sender, GroupSize);
    }

    public void SetOperationType(object sender, OperationType operationType)
    {
        OperationType = operationType;
        OnOperationTypeChanged(sender, operationType);
    }   
    
    public void SetGuiType(object sender, GuiType guiType)
    {
        GuiType = guiType;
        OnGuiTypeChanged(sender, guiType);
    }

    private void OnExcelBookFilepathChanged(object sender, string value)
    {
        ExcelBookFilepathChanged?.Invoke(sender, value);
    }

    private void OnMaxWidthChanged(object sender, int? value)
    {
        MaxWidthChanged?.Invoke(sender, value);
    }

    private void OnMaxWasteChanged(object sender, double? value)
    {
        MaxWasteChanged?.Invoke(sender, value);
    }

    private void OnGroupSizeChanged(object sender, int? value)
    {
        GroupSizeChanged?.Invoke(sender, value);
    }

    private void OnOperationTypeChanged(object sender, OperationType value)
    {
        OperationTypeChanged?.Invoke(sender, value);
    }

    private void OnGuiTypeChanged(object sender, GuiType value)
    {
        GuiTypeChanged?.Invoke(sender, value);
    }
}