using EmberToolkit.Common.Interfaces.Repository;
using System;
using UnityEngine;

namespace EmberToolkit.Unity.Data
{
    public abstract class EmberObject : IEmberObject
    {
        [SerializeField] protected Guid _id;
        [SerializeField] protected string _name;
        public Guid Id => _id;

        public abstract Type ItemType { get; }

        public string Name => _name;

        public EmberObject()
        {
            _id = Guid.NewGuid();
            _name = string.Empty;
        }

        public EmberObject(string inputName)
        {
            _id = Guid.NewGuid();
            _name = inputName;
        }
        public EmberObject(Guid id, string inputName)
        {
            _id = id;
            _name = inputName;
        }
    }
}
