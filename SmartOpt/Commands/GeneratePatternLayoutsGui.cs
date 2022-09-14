using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Commands;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.ViewModels;
using SmartOpt.Modules.PatternLayoutsGenerator.UI.Views;

namespace SmartOpt;

public partial class Application
{
    private void GeneratePatternLayoutsGui(
        IPatternLayoutService patternLayoutService,
        IReportExporter reportExporter)
    {
        var viewModel = new MainWindowViewModel
        {
            WorkbookFilename = Path.GetFileName(_applicationState.ExcelBookFilepath!) ?? "Активная книга"
        };
        var window = new MainWindow
        {
            DataContext = viewModel
        };

        // ReSharper disable once AsyncVoidLambda
        viewModel.GeneratePatternLayouts = new RelayCommand(async _ =>
            {
                _applicationState.SetMaxWidth(viewModel, viewModel.MaxWidth);
                _applicationState.SetMaxWaste(viewModel, viewModel.MaxWaste);
                _applicationState.SetGroupSize(viewModel, viewModel.GroupSize);

                // viewModel.IsInteractionAllowed = false;
                viewModel.BusyIndicatorManager.Show(1, "Обрабатываем...");

                Task task = TaskEx.Run(() => GeneratePatternLayoutsNoGui(patternLayoutService, reportExporter));
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        exception.InnerException != null ? exception.InnerException.Message : exception.Message,
                        "An unexpected error was occured",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                finally
                {
                    viewModel.BusyIndicatorManager.Close(1);
                    // viewModel.IsInteractionAllowed = true;
                }
            },
            CanExecuteGeneratePattenLayoutsCommand(viewModel));
        viewModel.IncrementGroupSize = new RelayCommand(_ => viewModel.GroupSize++);
        viewModel.DecrementGroupSize = new RelayCommand(_ => viewModel.GroupSize--);

        window.ShowDialog();
    }

    private Func<object, bool> CanExecuteGeneratePattenLayoutsCommand(IMainWindowViewModel viewModel) =>
        _ => viewModel.MaxWidth >= 0 && viewModel.MaxWaste > 0 && viewModel.GroupSize > 0;
}