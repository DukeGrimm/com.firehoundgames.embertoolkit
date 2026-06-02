using EmberToolkit.Common.Interfaces.Repository;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace EmberToolkit.Common.ScriptableObjects.Base
{
    public class ScriptableEmber : SerializedScriptableObject, IEmberObject
    {
            public Guid Id => _id;
            [ValidateInput("ValidateGuid", "Guid is empty!", InfoMessageType.Error)]
            [OdinSerialize] protected Guid _id;

            public ScriptableEmber() : base()
            {
                _id = Guid.NewGuid();
            }
            public ScriptableEmber(Guid itemID) : base()
            {
                _id = itemID;
            }

            private bool ValidateGuid(Guid guid)
            {
                return guid != Guid.Empty;
            }
    }
}
