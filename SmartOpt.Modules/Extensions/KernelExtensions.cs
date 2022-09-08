using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using SmartOpt.Core.Infrastructure.Interfaces;

namespace SmartOpt.Modules.Extensions;

public static class KernelExtensions
{
    public static void AddModules(this IKernel kernel, IEnumerable<Type> entryPointsAssembly)
    {
        var modules = new List<IModule>();
        foreach (Type entryPoint in entryPointsAssembly)
        {
            IEnumerable<Type> types = entryPoint.Assembly.GetExportedTypes().Where(x => !x.IsAbstract && typeof(IModule).IsAssignableFrom(x));
            IEnumerable<IModule> instances = types.Select(Activator.CreateInstance).Cast<IModule>();
            modules.AddRange(instances);
        }

        modules.ForEach(module => module.ConfigureServices(kernel));

        kernel.Bind<List<IModule>>().ToConstant(modules);
    }

    public static void AddApplicationState(this IKernel kernel, IApplicationState state)
    {
        kernel.Bind<IApplicationState>().ToConstant(state);
    }
}