using System;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface IEmberObject
    {
        Guid Id { get; }
        //Removed for simplicity
        //Type ItemType { get; }
        //string Name { get; }
    }
}
