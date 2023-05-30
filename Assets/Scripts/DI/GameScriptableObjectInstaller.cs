using SNShien.Common.AssetTools;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameScriptableObjectInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private GameSettingScriptableObject gameExternalSetting;
        [SerializeField] private PatternSettingScriptableObject patternSetting;
        [SerializeField] private FmodAudioCollectionScriptableObject audioCollection;
        [SerializeField] private AudioTriggerEventScriptableObject audioTriggerEventSetting;
        [SerializeField] private SceneProcessScriptableObject sceneProcessSetting;
        [SerializeField] private LoadAssetScriptableObject loadAssetSetting;

        public override void InstallBindings()
        {
            Container.BindInstance(patternSetting);
            Container.BindInstance<IAudioCollection>(audioCollection);
            Container.BindInstance<IAudioTriggerEventSetting>(audioTriggerEventSetting);
            Container.BindInstance<ISceneProcessSetting>(sceneProcessSetting);
            Container.Bind<IGameSetting>().FromInstance(gameExternalSetting);
            Container.Bind<IPatternSetting>().FromInstance(patternSetting);
            Container.Bind<ILoadAssetSetting>().FromInstance(loadAssetSetting);
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
        }
    }

}