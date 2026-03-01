using System;

namespace EmberToolkit.Common.Interfaces.Configuration
{
    public interface IEmberSettings
    {
        string FileRoot { get; }
        string SavePath { get; }
        string saveKey { get; }
        bool useEncryption { get; }

        Guid BehaviourRepoID { get; }
        Guid ObjectRepoID { get; }
    }
}
