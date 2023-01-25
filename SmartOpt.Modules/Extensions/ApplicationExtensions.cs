using System;
using System.Collections.Generic;
using SmartOpt.Core.Extensions;
using SmartOpt.Core.Infrastructure.Interfaces;

namespace SmartOpt.Modules.Extensions;

public static class ApplicationExtensions
{
    public static void InitializeModules(this IApplication application)
    {
        IServiceProvider provider = application.GetServicesProvider();
        var modules = provider.GetRequiredService<List<IModule>>();
        modules.ForEach(module => module.InitializeModule(provider));
    }
}