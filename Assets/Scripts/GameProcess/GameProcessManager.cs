using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private AudioControllerComponent audioController;
        [Inject] private CardManager cardManager;
        [Inject] private IGameSetting gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;

        private void Start()
        {
            SetEventRegister();
            audioController.SetupRegisterAudioEvent(eventRegister);
            cardManager.StarGame(gameExternalSetting.GetPairCount);
            audioController.SetupRegisterAudioEvent(cardManager.GetPresenterRegister);
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<StartGameEvent>(OnStartGame);
            eventRegister.Register<StartGameEvent>(OnStartGame);
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

        private void OnStartGame(StartGameEvent eventInfo)
        {
            HideAllCards();

            audioManager.SetParam(AudioConstKey.AUDIO_PARAM_FADE_IN_TIMES, 0);
            audioManager.Play(AudioConstKey.AUDIO_KEY_BGM_GAME);

            SetupCardView(eventInfo.CardPresenter);
        }
    }
}