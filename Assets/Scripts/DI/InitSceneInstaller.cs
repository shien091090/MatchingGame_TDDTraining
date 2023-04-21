using SNShien.Common.ProcessTools;
using SNShien.Common.TimeTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class InitSceneInstaller : MonoInstaller
    {
        [Inject] private IGameSetting gameExternalSetting;
        [SerializeField] private SceneProcessManager processManager;

        public override void InstallBindings()
        {
            Container.BindInstance(processManager);
            Container.BindInterfacesAndSelfTo<ArchitectureEventHandler>().AsSingle().NonLazy();
            Container.Bind<TimeAsyncExecuter>().AsSingle().NonLazy();
            Container.Bind<PointManager>().AsSingle().WithArguments(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint).NonLazy();
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}