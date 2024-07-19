using System;
using System.Collections.Generic;
using UnityEngine;
using EmberToolkit.Common.Interfaces.Settings;

namespace EmberToolkit.Unity.Behaviours.Managers
{

    public abstract class PlayerPrefsManagerBase : EmberSingleton, IPlayerPrefsManager
    {
        private IGraphicsOptions _graphicOptions;
        private IAudioManager _audioManager;

        private Dictionary<string, float> _mixerSettingsCache = new Dictionary<string, float>();
        private bool IsFullscreenCache = true;
        private int ResolutionIdIndex = 20;

        private static readonly string FULLESCREEN_KEY = "IsFullScreen";
        private static readonly string RESOLUTION_INDEX_KEY = "Resolution";


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            //TODO: Why does PlayerPrefs need to access the Graphics Options?
            RequestService(out _graphicOptions);
            RequestService(out _audioManager);
            ResolutionIdIndex = _graphicOptions.SelectedResolutionIndex;
        }

        public void SavePlayerPrefs()
        {
            SaveMixerSettings();
            SaveGraphicsSettings();
            PlayerPrefs.Save();
        }

        public void LoadPlayerPrefs()
        {
            LoadMixerSettings();
            LoadGraphicsSettings();
        }

        #region VolumeSettings

        public float GetVolumeMixerSetting(string mixerName) => PlayerPrefs.GetFloat(mixerName, 1.0f);
        public void SetMixerSetting(string mixerName, float newSetting)
        {
            if (_mixerSettingsCache.ContainsKey(mixerName)) { _mixerSettingsCache[mixerName] = newSetting; }
            else _mixerSettingsCache.Add(mixerName, newSetting);
        }
        public Dictionary<string, float> GetAllVolumeSettings()
        {
            if(_mixerSettingsCache?.Count == 0)
            {
                LoadMixerSettings();
            }
            return _mixerSettingsCache;
        }

        public void SaveMixerSettings()
        {
            foreach (KeyValuePair<string, float> mixerDic in _mixerSettingsCache)
            {
                PlayerPrefs.SetFloat(mixerDic.Key, mixerDic.Value);
            }
        }
        private void LoadMixerSettings()
        {
            _mixerSettingsCache.Clear();
            //TODO: Rework to get get value without needing Enum

            foreach (string mixer in _audioManager.GetMixerNames())
            {
                _mixerSettingsCache.Add(mixer, PlayerPrefs.GetFloat(mixer.ToString()));

            }
        }

        #endregion
        #region GraphicsSettings
        private void SaveGraphicsSettings()
        {
            SaveFullscreenMode();
            SaveResolutionSetting();
        }
        private void LoadGraphicsSettings()
        {
            LoadFullscreenMode();
            LoadResolutionSetting();
        }
        public void SetFullescreenMode(bool mode) => IsFullscreenCache = mode;
        private void SaveFullscreenMode() => PlayerPrefs.SetInt(FULLESCREEN_KEY, IsFullscreenCache ? 1 : 0);
        private void LoadFullscreenMode() => IsFullscreenCache = PlayerPrefs.GetInt(FULLESCREEN_KEY, 1) == 1;
        public bool GetFullscreenMode() {
            return  IsFullscreenCache;
        }

        public void SetResolutionSetting(int index) => ResolutionIdIndex = index;
        private void SaveResolutionSetting() => PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, ResolutionIdIndex);
        private void LoadResolutionSetting() => ResolutionIdIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, ResolutionIdIndex);
        public int GetResolutionSetting() {
            return ResolutionIdIndex; 
        }


            #endregion


        }
}
