using System.ComponentModel;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Annotations;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}