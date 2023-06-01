using SNShien.Common.AssetTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TimeTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class InitSceneInstaller : MonoInstaller
    {
        [SerializeField] private SceneProcessManager processManager;
        [SerializeField] private SceneProcessScriptableObject sceneProcessSetting;
        [SerializeField] private LoadAssetScriptableObject loadAssetSetting;

        public override void InstallBindings()
        {
            Container.Bind<ISceneProcessSetting>().FromInstance(sceneProcessSetting);
            Container.BindInstance(processManager);
            Container.BindInterfacesAndSelfTo<ArchitectureEventHandler>().AsSingle().NonLazy();
            Container.Bind<ILoadAssetSetting>().FromInstance(loadAssetSetting);
            Container.Bind<IAssetManager>().To<LoadAssetManager>().AsSingle().NonLazy();
            Container.Bind<TimeAsyncExecuter>().AsSingle().NonLazy();
        }
    }
}