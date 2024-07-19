using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Unity.Behaviours.Controllers;
using EmberToolkit.Integration.Configurator;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EmberToolkit.Integration.Extensions
{
    public static class EmberServiceExtensions
    {
        public static IServiceProvider Initializer(IEmberSettings settings)
        {
            //_provider = DependencyConfigurator.Initializer(settings, timeController);
            // Set up the dependency injection container
            IServiceProvider serviceProvider = new ServiceCollection()
                .ConfigureIntegrationServices(settings)
                .BuildServiceProvider();

            return serviceProvider;
        }
        public static IServiceCollection ConfigureIntegrationServices(this IServiceCollection services, IEmberSettings settings)
        {
            // Configure EmberToolkit services
            DependencyConfigurator.Configure(services, settings);
            return services;
        }
    }
}
