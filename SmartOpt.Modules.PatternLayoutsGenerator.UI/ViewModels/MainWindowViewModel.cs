using System.Windows.Input;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Commands;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private int _maxWidth;

    public MainWindowViewModel()
    {
        GenerateTemplateLayout = new GenerateTemplateLayoutCommand();
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

    public ICommand GenerateTemplateLayout { get; }
}
