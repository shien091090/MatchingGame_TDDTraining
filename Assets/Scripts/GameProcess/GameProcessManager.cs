using System.Collections.Generic;
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
        [Inject] private CardManager cardManager;
        [Inject] private GameSettingScriptableObject gameExternalSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;

        private void Start()
        {
            SetEventRegister();
            cardManager.StarGame(gameExternalSetting.GetPairCount);
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<StartGameEvent>(OnStartGame);
            eventRegister.Register<StartGameEvent>(OnStartGame);
        }

        private void SetupCardPrefab(int cardNumber, CardPresenter presenter)
        {
            CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
            if (cardObj == null)
                return;

            cardObj.SetCardInfo(cardNumber, presenter);
            cardObj.Show();
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

            foreach (Card card in cardManager.GetAllCards)
            {
                CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
                if (cardObj == null)
                    continue;
                
                cardObj.SetCardInfo(card.number, eventInfo.CardPresenter);
                cardObj.Show();
            }
        }
    }
}