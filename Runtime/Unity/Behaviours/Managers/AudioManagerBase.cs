using EmberToolkit.Common.Enum.Settings.Audio;
using EmberToolkit.Common.Interfaces.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace EmberToolkit.Unity.Behaviours.Managers
{
    public abstract class AudioManagerBase : EmberSingleton, IAudioManager
    {
        public AudioMixer GameMixer;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }
        public void SetMixerSetting(string mixerName, float value) => GameMixer.SetFloat(mixerName, ConvertToAudioLog10(value));

        private float ConvertToAudioLog10(float value) => Mathf.Log10(value) * 20;

        public abstract string[] GetMixerNames();


    }
}
