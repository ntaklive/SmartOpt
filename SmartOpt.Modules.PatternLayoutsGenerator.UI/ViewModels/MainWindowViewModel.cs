using System.Windows.Input;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Services;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    private bool _isInteractionAllowed = true;
    private int _maxWidth = 6000;
    private double _maxWaste = 3.0;
    private int _groupSize = 5;
    private string? _workbookFilepath;

    public MainWindowViewModel()
    {
        BusyIndicatorManager = BusyIndicatorManager.Instance;
    }

    public bool IsInteractionAllowed
    {
        get => _isInteractionAllowed;
        set
        {
            _isInteractionAllowed = value;
            OnPropertyChanged(nameof(IsInteractionAllowed));
        }
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
        get => _workbookFilepath ?? "Не указано";
        set
        {
            _workbookFilepath = value;
            OnPropertyChanged(nameof(WorkbookFilename));
        }
    }

    public ICommand GeneratePatternLayouts { get; set; } = null!;
    public ICommand IncrementGroupSize { get; set; } = null!;
    public ICommand DecrementGroupSize { get; set; } = null!;
    public ICommand SelectWorkbookFilepath { get; set; } = null!;

    public BusyIndicatorManager BusyIndicatorManager { get; }
}