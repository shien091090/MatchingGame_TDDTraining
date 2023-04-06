using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.ArchitectureTools;
using UnityEngine;

namespace GameCore
{
    public class CardPresenter
    {
        private readonly List<Card> cards;
        private IEventInvoker eventInvoker;
        private IEventRegister eventRegister;
        private readonly IPatternSetting patternSetting;

        public CardPresenter(List<Card> allCards, IPatternSetting patternSetting)
        {
            cards = allCards;
            this.patternSetting = patternSetting;
            SetEventHandler();
        }

        public Sprite GetPatternSprite(int cardNumber)
        {
            Card card = cards.FirstOrDefault(x => x.number == cardNumber);
            return card == null ?
                null :
                patternSetting.GetPatternSprite(card.GetPattern);
        }

        public void SetEventInvoker(IEventInvoker eventInvoker)
        {
            this.eventInvoker = eventInvoker;
        }

        public void SendSwitchCardCoverStateEvent(int cardNumber, bool isCover)
        {
            eventInvoker.SendEvent(new SwitchCoverStateEvent(cardNumber, isCover));
        }

        public void SendCardMatchEvent(List<Card> matchCards)
        {
            List<int> numbers = matchCards.Select(x => x.number).ToList();
            eventInvoker.SendEvent(new CardMatchEvent(numbers));
        }

        public void RegisterEvent<T>(Action<T> eventAction) where T : IArchitectureEvent
        {
            eventRegister.Unregister(eventAction);
            eventRegister.Register(eventAction);
        }

        private void SetEventHandler()
        {
            ArchitectureEventHandler eventHandler = new ArchitectureEventHandler();
            eventInvoker = eventHandler;
            eventRegister = eventHandler;
        }
    }
}