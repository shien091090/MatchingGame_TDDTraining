using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MainInstaller : MonoInstaller, IPrefabInstantiate
    {
        [SerializeField] private GameSettingScriptableObject gameExternalSetting;
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private PatternSettingScriptableObject patternSetting;

        public GameObject InstantiateGameObject(GameObject prefabReference, Transform parentHolder)
        {
            return Container.InstantiatePrefab(prefabReference, parentHolder);
        }

        public override void InstallBindings()
        {
            Container.BindInstance(patternSetting);
            Container.BindInstance(objectPoolManager);
            Container.BindInstance(gameExternalSetting);
            Container.BindInstance(new PointManager(gameExternalSetting.GetSuccessIncreasePoint, gameExternalSetting.GetFailIncreasePoint));
            Container.BindInstance<IPrefabInstantiate>(this);
            objectPoolManager.SetPrefabInstantiateInterface(this);
            Container.Bind<CardManager>().AsSingle().NonLazy();
        }
    }
}