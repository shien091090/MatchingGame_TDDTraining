using SNShien.Common.AssetTools;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameProcessManager : MonoBehaviour
    {
        // [SerializeField] private AudioAutoTriggerComponent audioAutoTriggerControllerPrefab;
        // [SerializeField] private MatchingGameView matchingGameViewPrefab;
        [Inject] private CardManager cardManager;
        [Inject] private IGameSetting gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IAssetManager assetManager;
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
            AudioAutoTriggerComponent audioAutoTriggerControllerPrefab = assetManager.GetAsset<AudioAutoTriggerComponent>("AudioAutoTriggerController");
            audioAutoTriggerComponent = Instantiate(audioAutoTriggerControllerPrefab);
            MatchingGameView matchingGameViewPrefab = assetManager.GetAsset<MatchingGameView>("MatchingGameView");
            matchingGameView = Instantiate(matchingGameViewPrefab);
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