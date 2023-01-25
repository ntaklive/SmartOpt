using System;

namespace SmartOpt.Core.Infrastructure.Interfaces;

public interface IApplication
{
    public void Start();
    public IServiceProvider GetServicesProvider();
}