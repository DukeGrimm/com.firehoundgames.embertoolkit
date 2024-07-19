using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Encryption;
using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Serialization;
using EmberToolkit.DataManagement.Controllers;
using EmberToolkit.DataManagement.Repositories;
using EmberToolkit.DataManagement.Serializers;
using EmberToolkit.Encryption.Controller;
using EmberToolkit.Encryption.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EmberToolkit.Integration.Configurator
{
    public static class DependencyConfigurator
    {
        public static void Configure(IServiceCollection services,IEmberSettings settings)
        {
            // Register your dependencies here

            //// Example: Register a service interface with its implementation
            //services.AddScoped<IMyService, MyService>();

            //// Example: Register a singleton
            services.AddSingleton(settings);
            services.AddSingleton<IAESController, AESController>();
            services.AddSingleton<IDataSerializer, EmberJsonSerializer>(); //"C:\\temp\\Personal\\SaveData"
            services.AddSingleton<IDataRepository, DataRepository>();
            services.AddSingleton<SaveLoadController>().AddInterfaces<SaveLoadController>();
            services.AddSingleton<SaveableObjectRepository>().AddInterfaces<SaveableObjectRepository>();
            services.AddSingleton<SaveableBehaviourRepository>().AddInterfaces<SaveableBehaviourRepository>();

        }
        /// <summary>
        /// Register all interfaces recursivly 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public static IServiceCollection AddInterfaces<T>(this IServiceCollection services)
        {
            var interfaces = typeof(T).GetInterfaces()
                .Where(interfaceType => interfaceType != typeof(IDisposable));

            foreach (var @interface in interfaces)
            {
                services.AddSingleton(@interface, provider => provider.GetRequiredService<T>());
            }

            return services;
        }
    }
}
