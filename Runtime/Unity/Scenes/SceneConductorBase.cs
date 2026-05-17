using EmberToolkit.Common.Interfaces.StateNodes;
using EmberToolkit.Common.Interfaces.Unity.Behaviours.Managers.StateNodes;
using EmberToolkit.Unity.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EmberToolkit.Unity.Scenes
{
    public abstract class SceneConductorBase<E> : EmberSingleton
    {
        protected IStateNodeManager<E> _stateManager;
        protected IStateNode<E> _sceneStateNode;

        [SerializeField] protected Selectable UI_SelectedOnSceneLoad;

        protected override void Awake()
        {
            base.Awake();
            RequestService(out _stateManager);
            //InitEventSubscriptions();
            if (UI_SelectedOnSceneLoad != null) SelectOnLoadUIElement();
        }

        protected virtual void SelectOnLoadUIElement()
        {
            if (UI_SelectedOnSceneLoad != null) UI_SelectedOnSceneLoad.Select();
        }

        protected void LoadScene(E gameScene)
        {
            SceneManager.LoadScene(gameScene.ToString(), LoadSceneMode.Single);
        }
    }
}
