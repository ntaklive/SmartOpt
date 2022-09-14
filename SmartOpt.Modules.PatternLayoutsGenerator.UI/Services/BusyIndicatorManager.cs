using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Annotations;

namespace SmartOpt.Modules.PatternLayoutsGenerator.UI.Services;

public class BusyIndicatorManager : INotifyPropertyChanged
{
    private static readonly object SyncRoot = new();
    private static BusyIndicatorManager? _instance;
    private readonly Dictionary<int, string> _busyParameters;

    private bool _isBusy;
    private string _message;

    private BusyIndicatorManager()
    {
        _isBusy = false;
        _message = string.Empty;
        _busyParameters = new Dictionary<int, string>();
    }

    public static BusyIndicatorManager Instance
    {
        get
        {
            lock (SyncRoot)
            {
                return _instance ??= new BusyIndicatorManager();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public string Message
    {
        get => _message;
        private set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Show(int id, string busyMessage)
    {
        if (!_busyParameters.ContainsKey(id))
        {
            _busyParameters.Add(id, busyMessage);
            IsBusy = true;
            Message = busyMessage;
        }
        else
        {
            _busyParameters[id] = busyMessage;
            IsBusy = true;
            Message = busyMessage;
        }
    }

    public void Close(int id)
    {
        if (_busyParameters.ContainsKey(id))
        {
            _busyParameters.Remove(id);
        }

        if (_busyParameters.Count == 0)
        {
            IsBusy = false;
            Message = string.Empty;
        }
        else
        {
            IsBusy = true;
            Message = _busyParameters.Last().Value;
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}