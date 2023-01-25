using System;
using System.Linq;
using System.Text;
using Ninject;
using SmartOpt.Core.Infrastructure;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;
using SmartOpt.Exceptions;
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
        Win32.HideConsole();
        
        if (args != null && args.Length > 0)
        {
            try
            {
                HandleArguments(args);
            }
            catch (HelpIsShownException)
            {
                return;
            }
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

        if (argumentsParser.TryParseHelp(aggregatedArgs))
        {
            PrintHelp();
            throw new HelpIsShownException();
        }
        
        if (argumentsParser.TryParseGuiType(aggregatedArgs, out GuiType guiType))
        {
            ApplicationState.SetGuiType(null!, guiType);
        }

        if (!argumentsParser.TryParseOperationType(aggregatedArgs, out OperationType operationType))
        {
            return;
        }

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

    private static void ThrowInvalidArgumentValueIfNoGui(string argumentName)
    {
        if (ApplicationState.GuiType == GuiType.NoGui)
        {
            throw new InvalidOperationException($"The '{argumentName}' argument has an invalid value");
        }
    }

    private static void PrintHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("'Run as' configuration");
        sb.AppendLine("-gui\t\t\t\t\t run application as an interactable window");
        sb.AppendLine("-noGui\t\t\t\t\t run application as CLI");
        sb.AppendLine("");
        
        sb.AppendLine("'Operation type' configuration");
        sb.AppendLine("-op %name%\t\t\t\t operation type. Available: generatePatternLayouts");
        sb.AppendLine("");

        
        sb.AppendLine("'Pattern Layouts Generator' configuration (-op generatePatternLayouts)");
        sb.AppendLine("-workbookFilepath '%filepath%.xlsm|xlsx' specify a filepath to the required excel workbook");
        sb.AppendLine("-maxWidth %value%\t\t\t specify wax width");
        sb.AppendLine("-groupSize %value%\t\t\t specify group size");
        sb.AppendLine("-maxWaste %value%\t\t\t specify max waste");
        sb.AppendLine("");

        Console.Write(sb.ToString());
    }
}