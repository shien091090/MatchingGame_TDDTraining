using Zenject;

namespace GameCore
{
    public class MainInstaller : MonoInstaller
    {
        [Inject] private GameSettingScriptableObject gameExternalSetting;

        public override void InstallBindings()
        {
            Container.Bind<PointManager>().AsSingle().WithArguments(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint).NonLazy();
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}