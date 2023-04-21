using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.ProcessTools;
using SNShien.Common.TimeTools;
using UnityEngine;

namespace GameCore
{
    public class CardPresenter
    {
        private IEventInvoker eventInvoker;
        private TimeAsyncExecuter timeAsyncExecuter;
        private readonly List<Card> cards;
        private readonly IPatternSetting patternSetting;
        private readonly IGameSetting gameSetting;

        public IEventRegister GetEventRegister { get; private set; }

        public CardPresenter(List<Card> allCards, IPatternSetting patternSetting, IGameSetting gameSetting, TimeAsyncExecuter timeAsyncExecuter)
        {
            cards = allCards;
            this.patternSetting = patternSetting;
            this.gameSetting = gameSetting;
            this.timeAsyncExecuter = timeAsyncExecuter;
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

        public void SendSwitchCardCoverStateEvent(int cardNumber, bool isCover, bool isDelay)
        {
            if (isDelay)
                timeAsyncExecuter.DelayedCall(gameSetting.GetCardDelayCoverTimes, () =>
                {
                    eventInvoker.SendEvent(new SwitchCoverStateEvent(cardNumber, isCover));
                });
            else
            {
                eventInvoker.SendEvent(new SwitchCoverStateEvent(cardNumber, isCover));
            }
        }

        public void SendCardMatchEvent(List<Card> matchCards)
        {
            List<int> numbers = matchCards.Select(x => x.number).ToList();
            timeAsyncExecuter.DelayedCall(gameSetting.GetCardDelayCoverTimes, () =>
            {
                eventInvoker.SendEvent(new PlayCardMatchEffectEvent(numbers));
            });
        }

        public void RegisterEvent<T>(Action<T> eventAction) where T : IArchitectureEvent
        {
            GetEventRegister.Unregister(eventAction);
            GetEventRegister.Register(eventAction);
        }

        public void LockCardAndUnlockAfterDelay(MatchType matchResult)
        {
            eventInvoker.SendEvent(new RefreshButtonFrozeStateEvent(true));
            float waitTimes;

            switch (matchResult)
            {
                case MatchType.Match:
                case MatchType.None:
                case MatchType.MatchAndGameFinish:
                case MatchType.WaitForNextCard:
                    waitTimes = gameSetting.NormalFrozeTimes;
                    break;

                case MatchType.NotMatch:
                    waitTimes = gameSetting.NotMatchFrozeTimes;
                    break;

                default:
                    return;
            }

            timeAsyncExecuter.DelayedCall(waitTimes, () =>
            {
                eventInvoker.SendEvent(new RefreshButtonFrozeStateEvent(false));
            });
        }

        private void SetEventHandler()
        {
            ArchitectureEventHandler eventHandler = new ArchitectureEventHandler();
            eventInvoker = eventHandler;
            GetEventRegister = eventHandler;
        }
    }
}