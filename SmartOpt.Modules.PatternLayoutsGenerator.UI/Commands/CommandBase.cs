using System;
using System.Windows.Input;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.Commands;

public abstract class CommandBase : ICommand
{
    public event EventHandler CanExecuteChanged;

    public abstract void Execute(object parameter);

    public virtual bool CanExecute(object parameter) => true;

    protected virtual void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}