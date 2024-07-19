using EmberToolkit.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Common.Interfaces.Data
{
    public delegate void SaveLoadEventHandler(IDataRepository dataRepository);
    public interface ISaveLoadEvents
    {
        Guid Id { get; }
        //Events
        event SaveLoadEventHandler OnSaveEvent;
        event SaveLoadEventHandler OnLoadEvent;
    }
}
