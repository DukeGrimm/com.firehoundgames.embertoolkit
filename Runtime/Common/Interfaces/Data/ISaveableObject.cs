using EmberToolkit.Common.Interfaces.Repository;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface ISaveableObject : IEmberObject
    {
        void Save(IDataRepository repo);
        void Load(IDataRepository repo);
        void MapFromPayload(object source);
    }
}
