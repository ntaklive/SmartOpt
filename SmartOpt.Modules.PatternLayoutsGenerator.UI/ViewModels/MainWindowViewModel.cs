using System.Windows.Input;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    private int _maxWidth = 6000;
    private double _maxWaste = 3.0;
    private int _groupSize = 5;
    private string? _workbookFilepath;

    public MainWindowViewModel()
    {
    }

    public int MaxWidth
    {
        get => _maxWidth;
        set
        {
            _maxWidth = value;
            OnPropertyChanged(nameof(MaxWidth));
        }
    }

    public double MaxWaste
    {
        get => _maxWaste;
        set
        {
            _maxWaste = value;
            OnPropertyChanged(nameof(MaxWaste));
        }
    }

    public int GroupSize
    {
        get => _groupSize;
        set
        {
            _groupSize = value;
            OnPropertyChanged(nameof(GroupSize));
        }
    }    
    
    public string? WorkbookFilename
    {
        get => _workbookFilepath ?? "Active workbook";
        set
        {
            _workbookFilepath = value;
            OnPropertyChanged(nameof(WorkbookFilename));
        }
    }

    public ICommand GeneratePatternLayouts { get; set; } = null!;
    public ICommand IncrementGroupSize { get; set; } = null!;
    public ICommand DecrementGroupSize { get; set; } = null!;
}