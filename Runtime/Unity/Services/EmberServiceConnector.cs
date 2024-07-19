using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Services;
using EmberToolkit.Integration.Extensions;
using System;

namespace EmberToolkit.Unity.Services
{
    public class EmberServiceConnector : IEmberServiceConnector
    {
        protected IServiceProvider _provider;

        protected void InitilizeServiceConductor() => ServiceConductor.SetServiceConnector(this);

        public EmberServiceConnector(IEmberSettings settings)
        {
            // Set up the dependency injection container
            _provider = EmberServiceExtensions.Initializer(settings);

            // Resolve and use services
            InitilizeServiceConductor();
        }
        public virtual T GetCoreService<T>()
        {
            if (_provider == null) return default(T);

            return (T)_provider.GetService(typeof(T));
        }

        public virtual object? GetCoreService(Type serviceType)
        {
            if (_provider == null) return default(object);

            return _provider.GetService(serviceType);
        }
    }
}
