using EmberToolkit.Common.Interfaces.Data;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.DataManagement.Data
{
    public class StorageContainer : IStorageContainer
    {
        [OdinSerialize]
        public Dictionary<Type, object> RepositoryContainer { get; set; } = new Dictionary<Type, object>();
    }
}
