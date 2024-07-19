using EmberToolkit.Common.Enum.Settings.Audio;
using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Settings
{
    public interface IPlayerPrefsManager
    {

        void SavePlayerPrefs();
        void LoadPlayerPrefs();

        float GetVolumeMixerSetting(string mixerName);
        void SetMixerSetting(string mixerName, float newSetting);
        Dictionary<string, float> GetAllVolumeSettings();
        void SetFullescreenMode(bool mode);
        bool GetFullscreenMode();
        void SetResolutionSetting(int index);
        int GetResolutionSetting();


    }
}
