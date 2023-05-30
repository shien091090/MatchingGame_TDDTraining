using SNShien.Common.AssetTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [Inject] private IAssetManager assetManager;
        [Inject] private IEventInvoker eventInvoker;

        private void Start()
        {
            assetManager.OnAllAssetLoadCompleted -= SwitchToNextScene;
            assetManager.OnAllAssetLoadCompleted += SwitchToNextScene;
            assetManager.LoadAsset();
        }

        private void SwitchToNextScene()
        {
            assetManager.OnAllAssetLoadCompleted -= SwitchToNextScene;
            eventInvoker.SendEvent(new SwitchSceneEvent("EnterGame"));
        }
    }
}