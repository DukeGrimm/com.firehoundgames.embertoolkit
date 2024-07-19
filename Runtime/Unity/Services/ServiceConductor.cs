using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Services;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmberToolkit.Unity.Services
{
    public static class ServiceConductor
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        private static IEmberServiceConnector _connector;


        public static void SetServiceConnector(IEmberServiceConnector connector) => _connector = connector;

        private static bool ConnectorIsInitilized => _connector != null;


        public static void Register<T>(T service, Type serviceType = null)
        {
            //if (_conductor == null && serviceType == typeof(ServiceConductorOld)) _conductor = service as ServiceConductorOld;
            if (serviceType == null) serviceType = typeof(T);
            // Automatically register implemented interfaces and their derived interfaces
            RegisterInterfaces(serviceType, service);
        }

        private static void RegisterInterfaces(Type serviceType, object service)
        {
            foreach (Type interfaceType in serviceType.GetInterfaces())
            {
                if (interfaceType != typeof(ISaveableObject) 
                    && interfaceType != typeof(IEmberBehaviour) 
                    && interfaceType != typeof(IEmberObject)
                    && interfaceType != typeof(ISerializationCallbackReceiver) 
                    && interfaceType != typeof(ISupportsPrefabSerialization))
                {
                    services[interfaceType] = service;

                    // Check if the interface has derived interfaces and register them recursively
                    RegisterInterfaces(interfaceType, service);
                }
            }
        }

        private static void DeregisterInterfaces(Type serviceType, object service)
        {
            foreach (Type interfaceType in serviceType.GetInterfaces())
            {
                //services[interfaceType] = service;
                services.Remove(interfaceType);
                // Check if the interface has derived interfaces and register them recursively
                DeregisterInterfaces(interfaceType, service);
            }
        }

        public static T Get<T>()
        {
            if (services.TryGetValue(typeof(T), out object service) && service!= null)
            {
                return (T)service;
            }
            else
            {
                return GetFromCoreServices<T>();
            }

            // Handle case when requested service is not found
            return default(T);
        }

        /// <summary>
        /// Checks if a service or interface is registered.
        /// </summary>
        /// <typeparam name="T">The top-most interface or service type.</typeparam>
        /// <returns><c>true</c> if the service or interface is registered; otherwise, <c>false</c>.</returns>
        public static bool CheckForService<T>(bool checkCore = false)
        {
            bool found = false;
            var targetType = typeof(T);
            found = services.ContainsKey(targetType);
            if (found) return found;
            foreach (Type interfaceType in targetType.GetInterfaces())
            {
                found = services.ContainsKey(targetType);
                if (found) return found;
                found = CheckForService(interfaceType);
                if (found) return found;
            }
            if (checkCore) return IsCoreService<T>();
            return found;
        }

        /// <summary>
        /// Check for existing services registered to this type including interfaces to insure singleton pattern.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool CheckForService(Type targetType, bool checkCore = false)
        {
            bool found = false;
            found = services.ContainsKey(targetType);
            if(found) return found;
            foreach (Type interfaceType in targetType.GetInterfaces())
            {
                found = services.ContainsKey(targetType);
                if (found) return found;
                 found = CheckForService(interfaceType);
                if(found) return found;
            }
            if (checkCore && _connector.GetCoreService(targetType) != null) return true;
            return found;
        }

        public static void Deregister<T>(T service)
        {
            Type serviceType = typeof(T);
            DeregisterInterfaces(serviceType, service);
        }

        public static void RegisterServicesFromProvider(IServiceProvider provider)
        {
            if (provider != null)
            {
                //var services = provider. // IService is your controller interface type
                foreach (var service in services)
                {
                    var serviceType = service.GetType();
                    services[serviceType] = service; // Assuming services is a dictionary or similar data structure in your Service Locator
                }
            }
        }

        public static T GetFromCoreServices<T>()
        {
            if (!ConnectorIsInitilized) return default(T);
            return _connector.GetCoreService<T>();
        }
        public static object? GetFromCoreServices(Type serviceType)
        {
            if (!ConnectorIsInitilized) return null;
            return _connector.GetCoreService(serviceType);
        }
        private static bool IsCoreService<T>()
        {
            if(!ConnectorIsInitilized) return false;
            return _connector.GetCoreService<T>() != null;
        }


    }
}
