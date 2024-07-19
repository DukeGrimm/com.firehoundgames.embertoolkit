using EmberToolkit.Common.Enum.Settings.Audio;

namespace EmberToolkit.Common.Interfaces.Settings
{
    public interface IAudioManager
    {
        void SetMixerSetting(string mixerName, float value);
        string[] GetMixerNames();
    }
}
