using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.ArchitectureTools;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class CardManager
    {
        private IPatternSetting patternSetting;
        private List<Card> floppingCards = new List<Card>();
        private List<int> patternPool;
        private int pairCount;
        private bool useShuffle;
        private readonly PointManager pointManager;
        private readonly IEventInvoker eventInvoker;

        public int GetTotalCoveredCardCount => GetAllCards.Count(x => x.IsCovered);
        public List<Card> GetAllCards { get; private set; }

        public CardManager(IPatternSetting patternSetting, IEventInvoker eventHandler, PointManager pointManager = null)
        {
            this.pointManager = pointManager;
            this.patternSetting = patternSetting;
            eventInvoker = eventHandler;
        }

        private void InitPatternPool()
        {
            patternPool = new List<int>();
            patternPool.AddRange(patternSetting.GetPatternNumberList());
        }

        public void StarGame(int pairCount, bool useShuffle = true)
        {
            InitPatternPool();

            this.pairCount = pairCount;
            this.useShuffle = useShuffle;
            GetAllCards = new List<Card>();

            int number = 0;
            for (int i = 0; i < pairCount; i++)
            {
                int patternNumber = GetRandomPatternNumber();
                for (int j = 0; j < 2; j++)
                {
                    GetAllCards.Add(new Card(patternNumber, number));
                    number++;
                }
            }

            if (useShuffle)
                Shuffle();

            pointManager?.Reset();
            eventInvoker.SendEvent<StartGameEvent>();
            
        }

        public void Flop(int cardNumber, out MatchType matchResult)
        {
            matchResult = MatchType.None;

            Card selectCard = GetAllCards.FirstOrDefault(x => x.number == cardNumber);
            if (IsWrongSelect(selectCard))
            {
                matchResult = MatchType.WrongSelect;
                return;
            }

            selectCard.Flap();
            floppingCards.Add(selectCard);

            if (floppingCards.Count == 2)
            {
                if (CheckFloppingCardsIsMatch())
                {
                    FloppingCardsMatch();
                    ResetFloppingCards();
                    pointManager?.AddPoint();
                    matchResult = GetTotalCoveredCardCount == 0 ? MatchType.MatchAndGameFinish : MatchType.Match;
                }
                else
                {
                    CoverFloppingCards();
                    ResetFloppingCards();
                    pointManager?.SubtractPoint();
                    matchResult = MatchType.NotMatch;
                }
            }
            else
                matchResult = MatchType.WaitForNextCard;

            eventInvoker.SendEvent<FlopCardEvent>(matchResult);
        }

        public void RestartGame()
        {
            InitPatternPool();
            pointManager?.Reset();
            StarGame(pairCount, useShuffle);
        }

        private bool IsWrongSelect(Card selectCard)
        {
            return selectCard == null ||
                   selectCard.IsCovered == false ||
                   floppingCards.FirstOrDefault(x => x.number == selectCard.number) != null;
        }

        private int GetRandomPatternNumber()
        {
            int randomIndex = Random.Range(0, patternPool.Count);
            int patternNumber = patternPool[randomIndex];
            patternPool.RemoveAt(randomIndex);
            return patternNumber;
        }

        private bool CheckFloppingCardsIsMatch()
        {
            int card1Pattern = floppingCards[0].GetPattern;
            int card2Pattern = floppingCards[1].GetPattern;
            return card1Pattern == card2Pattern;
        }

        private void FloppingCardsMatch()
        {
            foreach (Card matchCard in floppingCards)
            {
                matchCard.SendMatchResult();
            }
        }

        private void ResetFloppingCards()
        {
            floppingCards = new List<Card>();
        }

        private void CoverFloppingCards()
        {
            foreach (Card floppingCard in floppingCards)
            {
                floppingCard.Cover();
            }
        }

        private void Shuffle()
        {
            List<Card> tempCardList = new List<Card>();
            tempCardList.AddRange(GetAllCards);
            List<Card> shuffledCards = new List<Card>();
            while (tempCardList.Count > 0)
            {
                int randomIndex = Random.Range(0, tempCardList.Count);
                shuffledCards.Add(tempCardList[randomIndex]);
                tempCardList.RemoveAt(randomIndex);
            }

            GetAllCards = shuffledCards;
        }
    }
}