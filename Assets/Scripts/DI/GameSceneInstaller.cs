using SNShien.Common.ArchitectureTools;
using SNShien.Common.TimeTools;
using Zenject;

namespace GameCore
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Inject] private IGameSetting gameExternalSetting;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ArchitectureEventHandler>().AsSingle().NonLazy();
            Container.Bind<TimeAsyncExecuter>().AsSingle().NonLazy();
            Container.Bind<PointManager>().AsSingle().WithArguments(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint).NonLazy();
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}