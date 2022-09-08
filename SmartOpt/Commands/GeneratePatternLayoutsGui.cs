using System;
using System.IO;
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
            WorkbookFilename = Path.GetFileName(_applicationState.ExcelBookFilepath!)
        };
        var window = new MainWindow
        {
            DataContext = viewModel
        };
        
        viewModel.GeneratePatternLayouts = new RelayCommand(_ =>
            {
                _applicationState.SetMaxWidth(viewModel, viewModel.MaxWidth);
                _applicationState.SetMaxWaste(viewModel, viewModel.MaxWaste);
                _applicationState.SetGroupSize(viewModel, viewModel.GroupSize);

                try
                {
                    GeneratePatternLayoutsNoGui(patternLayoutService, reportExporter);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.InnerException != null ? exception.InnerException.Message : exception.Message,
                        "An unexpected error was occured",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            },
            _ => viewModel.MaxWidth >= 0 && viewModel.MaxWaste > 0 && viewModel.GroupSize > 0);
        viewModel.IncrementGroupSize = new RelayCommand(_ => viewModel.GroupSize++);
        viewModel.DecrementGroupSize = new RelayCommand(_ => viewModel.GroupSize--);

        window.ShowDialog();
    }
}