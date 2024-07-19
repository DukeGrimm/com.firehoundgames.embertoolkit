using EmberToolkit.Common.Interfaces.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmberToolkit.Unity.Behaviours.Managers
{
    public class GraphicsManager : EmberSingleton, IGraphicsManager, IGraphicsOptions
    {
        private List<Resolution> resolutions;
        private int curSelResIndex = 0;
        // Start is called before the first frame update

        public int SelectedResolutionIndex => curSelResIndex;
        public bool IsFullscreen => Screen.fullScreen;
        protected override void Awake()
        {
            base.Awake();
            resolutions = Screen.resolutions.ToList();
            curSelResIndex = resolutions.IndexOf(Screen.currentResolution);

        }

        public List<string> GetResolutionOptions()
        {
            List<string> options = new List<string>();
            foreach (Resolution resolution in resolutions)
            {
                string option = $"{resolution.width} x {resolution.height}";
                options.Add(option);
            }
            return options;
        }

        public void SetResolution(int index)
        {
            Resolution newRes = resolutions[index];
            Screen.SetResolution(newRes.width, newRes.height, IsFullscreen);
            curSelResIndex = index;
        }

        public void SetFullScreenMode(bool mode) => Screen.fullScreen = mode;

    }
}
