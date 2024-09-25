using SNShien.Common.AssetTools;
using SNShien.Common.AudioTools;
using Zenject;

namespace GameCore
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Inject] private IAssetManager assetManager;

        public override void InstallBindings()
        {
            GameSettingScriptableObject gameExternalSetting = assetManager.GetAsset<GameSettingScriptableObject>(AssetNameConst.GAME_SETTING);
            PatternSettingScriptableObject patternSetting = assetManager.GetAsset<PatternSettingScriptableObject>(AssetNameConst.PATTERN_SETTING);
            FmodAudioCollectionScriptableObject audioCollection = assetManager.GetAsset<FmodAudioCollectionScriptableObject>(AssetNameConst.AUDIO_COLLECTION_SETTING);
            AudioTriggerEventScriptableObject audioTriggerEventSetting =
                assetManager.GetAsset<AudioTriggerEventScriptableObject>(AssetNameConst.AUDIO_TRIGGER_EVENT_SETTING);

            Container.BindInstance(patternSetting);
            Container.Bind<IAudioCollection>().FromInstance(audioCollection);
            Container.Bind<IAudioTriggerEventSetting>().FromInstance(audioTriggerEventSetting);
            Container.Bind<IGameSetting>().FromInstance(gameExternalSetting);
            Container.Bind<IPatternSetting>().FromInstance(patternSetting);
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
            Container.Bind<PointManager>().AsSingle().WithArguments(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint).NonLazy();
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}