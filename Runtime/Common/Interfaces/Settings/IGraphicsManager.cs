using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Settings
{
    public interface IGraphicsManager
    {
        void SetFullScreenMode(bool mode);
        void SetResolution(int index);
    }
}