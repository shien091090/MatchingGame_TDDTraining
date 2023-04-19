using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameProcessManager : MonoBehaviour
    {
        private const string PREFAB_KEY = "CardView";

        [SerializeField] private ObjectPoolManager cardObjPool;
        [SerializeField] private AudioAutoTriggerComponent audioAutoTriggerComponent;
        [SerializeField] private MainCanvasAnimator mainCanvasAnimator;
        [SerializeField] private ResetButton resetButton;
        [Inject] private CardManager cardManager;
        [Inject] private IGameSetting gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;

        private void Start()
        {
            SetEventRegister();
            StartGame();
        }

        private void StartGame()
        {
            cardManager.StartGame(gameExternalSetting.GetPairCount);
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<StartGameEvent>(OnStartGame);
            eventRegister.Register<StartGameEvent>(OnStartGame);

            eventRegister.Unregister<FlopCardEvent>(OnFlopCard);
            eventRegister.Register<FlopCardEvent>(OnFlopCard);

            audioAutoTriggerComponent.SetupRegisterAudioEvent(eventRegister);
        }

        private void SetupCardView(CardPresenter presenter)
        {
            foreach (Card card in cardManager.GetAllCards)
            {
                CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
                if (cardObj == null)
                    continue;

                cardObj.SetCardInfo(card.number, presenter);
                cardObj.Show();
            }
        }

        private void HideAllCards()
        {
            cardObjPool.HideAllCards(PREFAB_KEY);
        }

        private void OnFlopCard(FlopCardEvent eventInfo)
        {
            if (eventInfo.MatchResult != MatchType.MatchAndGameFinish)
                return;

            resetButton.SetButtonEnable(false);
            mainCanvasAnimator.PlaySettleAnimation(() =>
            {
                resetButton.SetButtonEnable(true);
            });
        }

        private void OnStartGame(StartGameEvent eventInfo)
        {
            audioAutoTriggerComponent.SetupRegisterAudioEvent(cardManager.GetPresenterRegister);
            mainCanvasAnimator.PlayIdleAnimation();

            HideAllCards();

            audioManager.SetParam(AudioConstKey.AUDIO_PARAM_FADE_IN_TIMES, 0);
            audioManager.Play(AudioConstKey.AUDIO_KEY_BGM_GAME);

            SetupCardView(eventInfo.CardPresenter);
        }
    }
}