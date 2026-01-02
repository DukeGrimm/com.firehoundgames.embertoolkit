using EmberToolkit.Common.Interfaces.Repository;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface ISaveableObject : IEmberObject
    {
        void Save();
        void Load();
        void MapFromPayload(object source);
    }
}
