using System;
using System.Windows;
using SmartOpt.Core.Extensions;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

namespace SmartOpt;

public partial class Application : IApplication
{
    private readonly IServiceProvider _provider;
    private readonly IApplicationState _applicationState;

    public Application(IServiceProvider provider)
    {
        _provider = provider;
        _applicationState = _provider.GetRequiredService<IApplicationState>();
    }

    public void Start()
    {
        try
        {
            switch (_applicationState)
            {
                case {OperationType: OperationType.GeneratePatternLayouts, GuiType: GuiType.Gui}:
                    GeneratePatternLayoutsGui(
                        _provider.GetRequiredService<IPatternLayoutService>(),
                        _provider.GetRequiredService<IReportExporter>()
                    );
                    break;
                case {OperationType: OperationType.GeneratePatternLayouts, GuiType: GuiType.NoGui}:
                    GeneratePatternLayoutsNoGui(
                        _provider.GetRequiredService<IPatternLayoutService>(),
                        _provider.GetRequiredService<IReportExporter>()
                    );
                    break;
                default:
                    GeneratePatternLayoutsGui(
                        _provider.GetRequiredService<IPatternLayoutService>(),
                        _provider.GetRequiredService<IReportExporter>()
                    );
                    break;
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.InnerException != null ? exception.InnerException.Message : exception.Message,
                "A critical error was occured",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public IServiceProvider GetServicesProvider()
    {
        return _provider;
    }
}