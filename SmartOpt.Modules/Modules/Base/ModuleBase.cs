using System;
using Ninject.Syntax;

namespace SmartOpt.Modules;

/// <summary>
/// Base implementation for <see cref="IModule"/>
/// </summary>
public abstract class ModuleBase : IModule
{
    public virtual void ConfigureServices(IBindingRoot services)
    {
    }

    public virtual void InitializeModule(IServiceProvider provider)
    {
    }
}