using System;

namespace SmartOpt.Core.Extensions;

public static class ServiceProviderExtensions
{
    public static TService GetRequiredService<TService>(this IServiceProvider provider)
    {
        var service = (TService) provider.GetService(typeof(TService));
        if (service is null)
        {
            throw new InvalidOperationException($"Failed to resolve an object of type {typeof(TService)}");
        }

        return service;
    }
    
    public static TService GetService<TService>(this IServiceProvider provider)
    {
        var service = (TService) provider.GetService(typeof(TService));
        
        return service;
    }
}