using System;

namespace EmberToolkit.Common.Interfaces.Data
{
    public delegate void SaveLoadEventHandler();
    public interface ISaveLoadEvents
    {
        Guid Id { get; }
        //Events
        event SaveLoadEventHandler OnSaveEvent;
        event SaveLoadEventHandler OnLoadEvent;
    }
}
