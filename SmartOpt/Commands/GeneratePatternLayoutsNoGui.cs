using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt;

public partial class Application
{
    private void GeneratePatternLayoutsNoGui(
        IPatternLayoutService patternLayoutService,
        IReportExporter reportExporter)
    {
        Report report;
        if (_applicationState.ExcelBookFilepath != null)
        {
            report = patternLayoutService.GeneratePatternLayoutsFromExcelWorksheet(
                _applicationState.ExcelBookFilepath,
                _applicationState.MaxWidth!.Value,
                _applicationState.MaxWaste!.Value,
                _applicationState.GroupSize!.Value);
        }
        else
        {
            report = patternLayoutService.GeneratePatternLayoutsFromActiveExcelWorksheet(
                _applicationState.MaxWidth!.Value,
                _applicationState.MaxWaste!.Value,
                _applicationState.GroupSize!.Value);
        }

        reportExporter.ExportToNewExcelWorkbook(report);
    }
}