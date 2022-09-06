using System;
using SmartOpt.Core.Extensions;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt;

public class Application : IApplication
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
        switch (_applicationState.OperationType)
        {
            case OperationType.GeneratePatternLayouts:
                GeneratePatternLayouts(
                    _provider.GetRequiredService<IPatternLayoutService>(),
                    _provider.GetRequiredService<IOrderInfoMerger>(),
                    _provider.GetRequiredService<IReportService>(),
                    _provider.GetRequiredService<IReportExporter>(),
                    _provider.GetRequiredService<IOrderInfoParser>()
                    );
                break;
        }
    }

    public IServiceProvider GetServicesProvider()
    {
        return _provider;
    }

    private void GeneratePatternLayouts(
        IPatternLayoutService patternLayoutService,
        IOrderInfoMerger orderInfoMerger,
        IReportService reportService,
        IReportExporter reportExporter,
        IOrderInfoParser orderInfoParser)
    {
        var orders = orderInfoParser.ParseOrdersFromActiveExcelWorksheet();
        var mergedOrders = orderInfoMerger.MergeOrdersWithIdenticalWidth(orders);
        var patternLayouts = patternLayoutService.CreatePatternLayoutsFromOrders(mergedOrders,
            _applicationState.MaxWidth.Value, _applicationState.MaxWaste.Value, _applicationState.GroupSize.Value);
        reportExporter.ExportToExcel(new Report(patternLayouts));

        // var app = new App();
        // app.Run();
    }
}