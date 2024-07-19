using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using EmberToolkit.Unity.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EmberToolkit.Unity.Behaviours
{
    public class EmberSingleton : EmberBehaviour, IEmberBehaviour
    {
        protected bool isServiceRegistered = false;

        protected override void Awake()
        {

            base.Awake();
            SingletonPattern();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //Remove Object from ServicesLocator before Object is destroyed
            DeregisterService();
        }


            public void SingletonPattern()
        {
            if (!CheckForThisService())
            {
                RegisterThisService();
            }
            else
            {
                Debug.LogError("This Type is already Registered in the service Locator! This: " + this);
            }
        }



        public void RegisterService(Type serviceType)
        {
            ServiceConductor.Register(this, serviceType);
            isServiceRegistered = true;
        }
        public void RegisterThisService()
        {
            if (!CheckForThisService())
            {
                ServiceConductor.Register(this, this.GetType());
                isServiceRegistered = true;
            }
        }

        public void DeregisterService()
        {
            if (isServiceRegistered)
            {
                ServiceConductor.Deregister(this);
                isServiceRegistered = false;
            }
        }


    }
}
