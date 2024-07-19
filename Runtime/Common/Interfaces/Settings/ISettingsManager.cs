using EmberToolkit.Common.Enum.Settings.Audio;
using System;

namespace EmberToolkit.Common.Interfaces.Settings
{
    public interface ISettingsManager
    {
        event Action OnRevertChanges;
        void LoadSettingsFromPlayerPrefs();
        void RevertChanges();
        void SaveChanges();
        void SetMixerSetting(string mixerName, float value);
        float GetMixerSetting(string mixerName);
        void SetFullscreenMode(bool mode);
        void SetResolution(int index);
        bool GetFullscreenMode();
        int GetResolutionSetting();
    }
}