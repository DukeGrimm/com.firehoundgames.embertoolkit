using System;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface IEmberObject
    {
        Guid Id { get; }
        Type ItemType { get; }
        string Name { get; }
    }
}
