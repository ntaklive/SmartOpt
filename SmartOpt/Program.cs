using System;
using System.Linq;
using Ninject;
using SmartOpt.Core.Infrastructure;
using SmartOpt.Core.Infrastructure.Enums;
using SmartOpt.Core.Infrastructure.Interfaces;
using SmartOpt.Modules;
using SmartOpt.Modules.Extensions;
using SmartOpt.Modules.PatternLayoutsGenerator;

namespace SmartOpt;

public static class Program
{
    [STAThread]
    public static void Main(string[]? args)
    {
        IApplicationState? applicationState = null;
        if (args != null && args.Length > 0)
        {
            applicationState = new ApplicationState();
            var argumentsParser = new ArgumentsParser();

            string aggregatedArgs = args.Aggregate((s1, s2) => $"{s1} {s2}");

            if (argumentsParser.TryParseOperationType(aggregatedArgs, out OperationType operationType))
            {
                applicationState.SetOperationType(null!, operationType);

                if (operationType == OperationType.GeneratePatternLayouts)
                {
                    if (argumentsParser.TryParseExcelWorkbookFilepath(aggregatedArgs, out string workbookFilepath))
                    {
                        applicationState.SetExcelWorkbookFilepath(null!, workbookFilepath);
                    }
                    // todo: else get active worksheet

                    if (argumentsParser.TryParseGroupSize(aggregatedArgs, out int groupSize))
                    {
                        applicationState.SetGroupSize(null!, groupSize);
                    }
                    else
                    {
                        ThrowInvalidArgumentValue(nameof(groupSize));
                    }

                    if (argumentsParser.TryParseMaxWidth(aggregatedArgs, out int maxWidth))
                    {
                        applicationState.SetMaxWidth(null!, maxWidth);
                    }
                    else
                    {
                        ThrowInvalidArgumentValue(nameof(maxWidth));
                    }

                    if (argumentsParser.TryParseMaxWaste(aggregatedArgs, out double maxWaste))
                    {
                        applicationState.SetMaxWaste(null!, maxWaste);
                    }
                    else
                    {
                        ThrowInvalidArgumentValue(nameof(maxWaste));
                    }
                }
            }
        }

        var kernel = new StandardKernel();
        kernel.AddModules(new[]
        {
            typeof(CommonModule),
            typeof(PatternLayoutGeneratorModule)
        });
        kernel.AddApplicationState(applicationState ?? ApplicationState.Default);

        var app = new Application(kernel);
        app.InitializeModules();
        app.Start();
    }

    private static void ThrowInvalidArgumentValue(string argumentName)
    {
        throw new InvalidOperationException($"The '{argumentName}' argument has an invalid value");
    }
}