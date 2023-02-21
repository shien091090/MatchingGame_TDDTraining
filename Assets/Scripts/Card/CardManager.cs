using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class CardManager
    {
        private List<Card> floppingCards = new List<Card>();
        private readonly List<int> patternPool;
        private readonly PointManager pointManager;

        public int GetTotalCoveredCardCount => GetAllCards.Count(x => x.IsCovered);
        public List<Card> GetAllCards { get; private set; }

        public CardManager(PointManager pointManager = null)
        {
            this.pointManager = pointManager;
            patternPool = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
        }

        public void StarGame(int pairCount, bool useShuffle = true)
        {
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
                    CoverFloppingCards();
                    ResetFloppingCards();
                    pointManager?.SubtractPoint();
                    matchResult = MatchType.NotMatch;
                }
                else
                {
                    ResetFloppingCards();
                    pointManager?.AddPoint();
                    matchResult = GetTotalCoveredCardCount == 0 ? MatchType.MatchAndGameFinish : MatchType.Match;
                }
            }
            else
                matchResult = MatchType.WaitForNextCard;
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
            return card1Pattern != card2Pattern;
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