using SNShien.Common.ArchitectureTools;
using Zenject;

namespace GameCore
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Inject] private GameSettingScriptableObject gameExternalSetting;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ArchitectureEventHandler>().AsSingle().NonLazy();
            Container.Bind<PointManager>().AsSingle().WithArguments(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint).NonLazy();
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}