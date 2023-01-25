using System;
using System.Windows.Input;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.Commands;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
 
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
 
    public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
    }
 
    public bool CanExecute(object parameter)
    {
        return _canExecute(parameter);
    }
 
    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}