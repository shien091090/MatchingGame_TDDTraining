using FMODUnity;
using SNShien.Common.AssetTools;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameProcessManager : MonoBehaviour
    {
        [Inject] private CardManager cardManager;
        [Inject] private IGameSetting gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IAssetManager assetManager;
        [Inject] private IAudioCollection audioCollection;
        private MatchingGameView matchingGameView;
        private AudioAutoTriggerComponent audioAutoTriggerComponent;

        private void Start()
        {
            InitScene();
            SetEventRegister();
            StartGame();
        }

        private void StartGame()
        {
            cardManager.StartGame(gameExternalSetting.GetPairCount);
        }

        private void InitScene()
        {
            AudioAutoTriggerComponent audioAutoTriggerControllerPrefab = assetManager.GetAsset<AudioAutoTriggerComponent>(AssetNameConst.AUDIO_AUTO_TRIGGER_CONTROLLER);
            audioAutoTriggerComponent = Instantiate(audioAutoTriggerControllerPrefab, transform);

            audioManager.InitCollectionFromBundle(audioCollection, FmodAudioInitType.FromSetting);
            
            MatchingGameView matchingGameViewPrefab = assetManager.GetAsset<MatchingGameView>(AssetNameConst.MATCHING_GAME_VIEW);
            matchingGameView = Instantiate(matchingGameViewPrefab, transform);
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<StartGameEvent>(OnStartGame);
            eventRegister.Register<StartGameEvent>(OnStartGame);


            audioAutoTriggerComponent.SetupRegisterAudioEvent(eventRegister);
        }

        private void HideAllCards()
        {
            matchingGameView.HideAllCards();
        }

        private void OnStartGame(StartGameEvent eventInfo)
        {
            audioAutoTriggerComponent.SetupRegisterAudioEvent(cardManager.GetPresenterRegister);
            matchingGameView.PlayIdleAnimation();

            HideAllCards();

            audioManager.SetParam(AudioConstKey.AUDIO_PARAM_FADE_IN_TIMES, 0);
            audioManager.Play(AudioConstKey.AUDIO_KEY_BGM_GAME);

            matchingGameView.CreateCardPrefab(eventInfo.CardPresenter);
        }
    }
}