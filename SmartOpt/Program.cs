using System;
using System.Linq;
using Ninject;
using SmartOpt.Core.Infrastructure;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;
using SmartOpt.Modules;
using SmartOpt.Modules.Extensions;
using SmartOpt.Modules.PatternLayoutsGenerator;
using SmartOpt.Modules.PatternLayoutsGenerator.UI;

namespace SmartOpt;

public static class Program
{
    private static readonly IApplicationState ApplicationState = new ApplicationState();
    
    [STAThread]
    public static void Main(string[]? args)
    {
        if (args != null && args.Length > 0)
        {
            HandleArguments(args);
        }

        var kernel = new StandardKernel();
        kernel.AddModules(new[]
        {
            typeof(CommonModule),
            typeof(PatternLayoutGeneratorModule),
            typeof(PatternLayoutGeneratorUiModule)
        });
        kernel.AddApplicationState(ApplicationState);

        var app = new Application(kernel);
        app.InitializeModules();
        app.Start();
    }

    private static void HandleArguments(string[] args)
    {
        var argumentsParser = new ArgumentsParser();

        string aggregatedArgs = args.Aggregate((s1, s2) => $"{s1} {s2}");

        if (argumentsParser.TryParseGuiType(aggregatedArgs, out GuiType guiType))
        {
            ApplicationState.SetGuiType(null!, guiType);
        }
            
        if (argumentsParser.TryParseOperationType(aggregatedArgs, out OperationType operationType))
        {
            ApplicationState.SetOperationType(null!, operationType);

            if (operationType == OperationType.GeneratePatternLayouts)
            {
                if (argumentsParser.TryParseExcelWorkbookFilepath(aggregatedArgs, out string workbookFilepath))
                {
                    ApplicationState.SetExcelWorkbookFilepath(null!, workbookFilepath);
                }

                if (argumentsParser.TryParseGroupSize(aggregatedArgs, out int groupSize))
                {
                    ApplicationState.SetGroupSize(null!, groupSize);
                }
                else
                {
                    ThrowInvalidArgumentValueIfNoGui(nameof(groupSize));
                }

                if (argumentsParser.TryParseMaxWidth(aggregatedArgs, out int maxWidth))
                {
                    ApplicationState.SetMaxWidth(null!, maxWidth);
                }
                else
                {
                    ThrowInvalidArgumentValueIfNoGui(nameof(maxWidth));
                }

                if (argumentsParser.TryParseMaxWaste(aggregatedArgs, out double maxWaste))
                {
                    ApplicationState.SetMaxWaste(null!, maxWaste);
                }
                else
                {
                    ThrowInvalidArgumentValueIfNoGui(nameof(maxWaste));
                }
            }
        }
    }

    private static void ThrowInvalidArgumentValueIfNoGui(string argumentName)
    {
        if (ApplicationState.GuiType == GuiType.NoGui)
        {
            throw new InvalidOperationException($"The '{argumentName}' argument has an invalid value");
        }
    }
}