using System.Windows.Input;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;

public interface IMainWindowViewModel
{
    public int MaxWidth { get; set; }
    public double MaxWaste { get; set; }
    public int GroupSize { get; set; }
    public string? WorkbookFilename { get; set; }

    public ICommand GeneratePatternLayouts { get; set; }
    public ICommand IncrementGroupSize { get; set; }
    public ICommand DecrementGroupSize { get; set; }
    public ICommand SelectWorkbookFilepath { get; set; }
}