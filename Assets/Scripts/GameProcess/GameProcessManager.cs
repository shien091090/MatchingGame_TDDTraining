using System.Collections.Generic;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class GameProcessManager : MonoBehaviour
    {
        private const string PREFAB_KEY = "CardView";

        [Inject] private CardManager cardManager;
        [Inject] private GameSettingScriptableObject gameExternalSetting;
        [Inject] private ObjectPoolManager cardObjPool;
        private List<CardView> cardList;

        private void Start()
        {
            SetEventRegister();
            cardManager.StarGame(gameExternalSetting.GetPairCount);
        }

        private void SetEventRegister()
        {
            cardManager.OnStartGame -= OnStartGame;
            cardManager.OnStartGame += OnStartGame;
        }

        private void SetupCardPrefab(Card card)
        {
            CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
            if (cardObj == null)
                return;

            cardObj.SetCardInfo(card);
            cardObj.Show();
        }

        private void HideAllCards()
        {
            cardObjPool.HideAllCards(PREFAB_KEY);
        }

        private void OnStartGame()
        {
            HideAllCards();
            List<Card> getAllCards = cardManager.GetAllCards;
            foreach (Card card in getAllCards)
            {
                SetupCardPrefab(card);
            }
        }
    }
}