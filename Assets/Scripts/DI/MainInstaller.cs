using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private GameSettingScriptableObject gameExternalSetting;
        [SerializeField] private ObjectPoolManager objectPoolManager;

        public override void InstallBindings()
        {
            Container.BindInstance(objectPoolManager);
            Container.BindInstance(gameExternalSetting);
            Container.BindInstance(new PointManager(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint));
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}