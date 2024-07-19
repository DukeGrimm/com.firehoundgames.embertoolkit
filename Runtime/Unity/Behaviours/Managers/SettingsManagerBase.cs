using EmberToolkit.Common.Enum.Settings.Audio;
using EmberToolkit.Common.Interfaces.Settings;
using EmberToolkit.Unity.Behaviours;
using System;
using System.Collections.Generic;

namespace EmberToolkit.Unity.Behaviours.Managers
{
    public abstract class SettingsManagerBase : EmberSingleton, ISettingsManager
    {
        private IPlayerPrefsManager _playerPrefs;
        private IAudioManager _audioManager;
        private IGraphicsManager _graphicsManager;
        // Start is called before the first frame update

        public event Action OnRevertChanges;

        protected override void Awake()
        {
            base.Awake();
            RequestService(out _playerPrefs);
            RequestService(out _audioManager);
            RequestService(out _graphicsManager);
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            LoadSettingsFromPlayerPrefs();
        }

        public void SaveChanges() => _playerPrefs.SavePlayerPrefs();

        //Does this need to be public?
        public void LoadSettingsFromPlayerPrefs()
        {
            _playerPrefs.LoadPlayerPrefs();
            LoadMixerSettings();
            LoadGraphicsSettings();
        }

        public void RevertChanges()
        {
            LoadSettingsFromPlayerPrefs();
            OnRevertChanges?.Invoke();
        }

        #region AudioSettings
        public float GetMixerSetting(string mixerName) => _playerPrefs.GetVolumeMixerSetting(mixerName);
        public void SetMixerSetting(string mixerName, float value)
        {
            ApplyMixerSetting(mixerName, value);
            _playerPrefs.SetMixerSetting(mixerName, value);
        }
        private void LoadMixerSettings()
        {
            var mixerSettings = _playerPrefs.GetAllVolumeSettings();
            foreach (var kvp in mixerSettings)
            {
                ApplyMixerSetting(kvp.Key, kvp.Value);
            }
        }
        private void ApplyMixerSetting(string mixerName, float value) => _audioManager.SetMixerSetting(mixerName, value);
        #endregion
        #region GraphicsSettings
        public void SetFullscreenMode(bool mode)
        {
            _graphicsManager.SetFullScreenMode(mode);
            //ToDO: Send to Player Prefs
        }

        public void SetResolution(int index)
        {
            _graphicsManager.SetResolution(index);
            _playerPrefs.SetResolutionSetting(index);
        }

        public bool GetFullscreenMode() => _playerPrefs.GetFullscreenMode();
        public int GetResolutionSetting() => _playerPrefs.GetResolutionSetting();
        private void LoadGraphicsSettings()
        {
            _graphicsManager.SetFullScreenMode(_playerPrefs.GetFullscreenMode());
            _graphicsManager.SetResolution(_playerPrefs.GetResolutionSetting());
        }
        #endregion


    }
}
