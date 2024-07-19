using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface IStorageContainer
    {
        public Dictionary<Type, object> RepositoryContainer { get; }
    }
}
