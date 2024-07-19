using EmberToolkit.Unity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmberToolkit.Unity.Behaviours.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void RegisterService<T>(this MonoBehaviour monoBehaviour, T instance)
        {
            ServiceConductor.Register(instance);
        }

        public static T GetService<T>(this MonoBehaviour monoBehaviour)
        {
            return ServiceConductor.Get<T>();
        }
        public static bool GetService<T>(this MonoBehaviour monoBehaviour, out T target)
        {
            target = ServiceConductor.Get<T>();
            return target != null;
        }

        public static void Deregister<T>(this MonoBehaviour monoBehaviour, T instance)
        {
            ServiceConductor.Deregister(instance);
        }

        #region Helpers

        public static List<GameObject> GetImmediateChildren(this MonoBehaviour mono)
        {
            List<GameObject> children = new List<GameObject>();

            for (int i = 0; i < mono.transform.childCount; i++)
            {
                GameObject childObject = mono.transform.GetChild(i).gameObject;
                children.Add(childObject);
            }

            return children;
        }


        #endregion


    }
}
