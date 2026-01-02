using EmberToolkit.Common.Interfaces.Repository;
using System;
using UnityEngine;

namespace EmberToolkit.Unity.Data
{
    public abstract class EmberObject : IEmberObject
    {
        [SerializeField] protected Guid _id;
        public Guid Id => _id;

        protected virtual Type ItemType => GetType();


        public EmberObject()
        {
            _id = Guid.NewGuid();
        }

        public EmberObject(string inputName)
        {
            _id = Guid.NewGuid();
        }
        public EmberObject(Guid id, string inputName)
        {
            _id = id;
        }
    }
}
