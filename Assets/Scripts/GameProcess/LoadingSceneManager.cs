using SNShien.Common.AssetTools;
using SNShien.Common.ProcessTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [Inject] private IAssetManager assetManager;
        [Inject] private IEventInvoker eventInvoker;
        [SerializeField] private Slider sld_loadingProgress;
        [SerializeField] private Text txt_loadingProgress;
        [SerializeField] private Text txt_loadingAssetHint;
        [SerializeField] private Text txt_loadingCaption;

        private void Start()
        {
            SetEventRegister(true);
            assetManager.StartLoadAsset();
        }

        private void OnUpdateLoadingProgress(LoadingProgress progressInfo)
        {
            RefreshLoadingHint(progressInfo);
        }

        private string ConvertLoadingCaptionText(int currentProgress)
        {
            int dotCount = currentProgress % 3 + 1;
            string dotText = new string('.', dotCount);
            return $"下載中{dotText}";
        }

        private void SetEventRegister(bool isListen)
        {
            assetManager.OnAllAssetLoadCompleted -= SwitchToNextScene;
            assetManager.OnUpdateLoadingProgress -= OnUpdateLoadingProgress;

            if (isListen)
            {
                assetManager.OnAllAssetLoadCompleted += SwitchToNextScene;
                assetManager.OnUpdateLoadingProgress += OnUpdateLoadingProgress;
            }
        }

        private void RefreshLoadingHint(LoadingProgress progressInfo)
        {
            sld_loadingProgress.value = progressInfo.GetCompletedPercent;
            txt_loadingProgress.text = progressInfo.GetCompletedPercentText;
            txt_loadingCaption.text = ConvertLoadingCaptionText(progressInfo.LoadedCount);
            txt_loadingAssetHint.text = progressInfo.AssetName;
        }

        private void SwitchToNextScene()
        {
            SetEventRegister(false);
            eventInvoker.SendEvent(new SwitchSceneEvent("EnterGame"));
        }
    }
}