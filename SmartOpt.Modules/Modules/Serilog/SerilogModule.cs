using System;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using Ninject.Syntax;
using Serilog;
using Serilog.Events;
using SmartOpt.Core;
using SmartOpt.Core.Extensions;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace SmartOpt.Modules;

public class SerilogModule : ModuleBase
{
    public override void ConfigureServices(IBindingRoot services)
    {
        services
            .Bind<ILoggerFactory>()
            .To<SerilogLoggerFactory>()
            .InSingletonScope();
        services
            .Bind<ILogger>()
            .ToMethod(context => context.Kernel.GetRequiredService<ILoggerFactory>().GetCurrentClassLogger())
            .InTransientScope();
    }

    public override void InitializeModule(IServiceProvider provider)
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Verbose
            )
            .WriteTo.RollingFile(
                pathFormat: $"{AssemblyInfo.AssemblyDirectoryPath}/log.txt",
                restrictedToMinimumLevel: LogEventLevel.Verbose
            );

        Log.Logger = loggerConfiguration.CreateLogger();
    }
}