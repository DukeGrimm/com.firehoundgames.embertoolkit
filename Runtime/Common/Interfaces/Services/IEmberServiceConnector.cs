using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Common.Interfaces.Services
{
    public interface IEmberServiceConnector
    {
        T GetCoreService<T>();
        object? GetCoreService(Type classType);

    }
}
