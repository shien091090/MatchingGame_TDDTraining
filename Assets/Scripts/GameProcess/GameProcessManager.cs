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
        [Inject] private PatternSettingScriptableObject patternSetting;
        private List<CardView> cardList;

        private void Start()
        {
            cardManager.StarGame(gameExternalSetting.GetPairCount);

            List<Card> getAllCards = cardManager.GetAllCards;
            foreach (Card card in getAllCards)
            {
                SetupCardPrefab(card);
            }
        }

        private void SetupCardPrefab(Card card)
        {
            CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
            if (cardObj == null)
                return;

            cardObj.SetCardInfo(card);
            cardObj.Show();
        }
    }
}