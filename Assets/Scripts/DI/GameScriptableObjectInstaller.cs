using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameScriptableObjectInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private GameSettingScriptableObject gameExternalSetting;
        [SerializeField] private PatternSettingScriptableObject patternSetting;
        [SerializeField] private FmodAudioCollectionScriptableObject audioCollection;

        public override void InstallBindings()
        {
            Container.BindInstance(gameExternalSetting);
            Container.BindInstance(patternSetting);
            Container.BindInstance<IAudioCollection>(audioCollection);
            Container.Bind<IPatternSetting>().FromInstance(patternSetting);
            Container.Bind<IAudioManager>().To<FmodAudioManager>().AsSingle();
        }
    }
}