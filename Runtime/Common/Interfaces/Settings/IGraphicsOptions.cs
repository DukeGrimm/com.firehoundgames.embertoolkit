using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Settings
{
    public interface IGraphicsOptions
    {
        bool IsFullscreen { get; }
        int SelectedResolutionIndex { get; }

        List<string> GetResolutionOptions();
    }
}