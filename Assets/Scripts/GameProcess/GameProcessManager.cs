using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameProcessManager : MonoBehaviour
    {
        [SerializeField] private AudioAutoTriggerComponent audioAutoTriggerControllerPrefab;
        [SerializeField] private MatchingGameView matchingGameViewPrefab;
        [Inject] private CardManager cardManager;
        [Inject] private IGameSetting gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;
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
            audioAutoTriggerComponent = Instantiate(audioAutoTriggerControllerPrefab);
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