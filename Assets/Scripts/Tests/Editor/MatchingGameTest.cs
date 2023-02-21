using System.Collections.Generic;
using NUnit.Framework;

namespace GameCore
{
    public class MatchingGameTest
    {
        [Test]
        [TestCase(3, 6)]
        [TestCase(7, 14)]
        //遊戲開始, 場上有N*N覆蓋的牌, 且每種牌兩兩一組
        public void game_start_and_all_card_covered(int pairCount, int expectedCount)
        {
            CardManager cardManager = new CardManager();
            cardManager.StarGame(pairCount);

            CoveredCardCountShouldBe(cardManager, expectedCount);
            AllCardsShouldBePair(cardManager);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌不同
        public void flop_two_card_and_not_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = new CardManager();
            cardManager.StarGame(8, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 2, MatchType.NotMatch);
            CoveredCardCountShouldBe(cardManager, 16);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌相同
        public void flop_two_card_and_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = new CardManager();
            cardManager.StarGame(4, false);

            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.Match);
            CoveredCardCountShouldBe(cardManager, 6);
        }

        [Test]
        //遊戲開始, 尚未掀牌, 積分為0
        public void start_game_and_point_is_zero()
        {
            PointManager pointManager = new PointManager();
            CardManager cardManager = new CardManager(pointManager);
            cardManager.StarGame(4, false);

            CurrentPointShouldBe(pointManager, 0);
        }

        [Test]
        [TestCase(3, 2, 9, 3)]
        [TestCase(2, 3, 6, 0)]
        [TestCase(4, 0, 12, 12)]
        //連續成功N次, 再連續失敗N次, 計算積分
        public void continuously_succeed_many_times_then_fail_many_times_then_calculate_point(int successIncreasePoint, int failPointDamage,
            int expectedFinalSucceedPoint, int expectedFinalFailedPoint)
        {
            PointManager pointManager = new PointManager(successIncreasePoint, failPointDamage);
            CardManager cardManager = new CardManager(pointManager);
            cardManager.StarGame(6, false);

            CurrentPointShouldBe(pointManager, 0);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.Match);

            CurrentPointShouldBe(pointManager, expectedFinalSucceedPoint);

            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);
            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);
            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);

            CurrentPointShouldBe(pointManager, expectedFinalFailedPoint);
        }

        [Test]
        [TestCase(3, 2, 5)]
        [TestCase(4, 5, 4)]
        //成功和失敗交叉發生, 計算積分
        public void success_and_fail_occur_alternately_then_calculate_point(int successIncreasePoint, int failPointDamage, int expectedFinalPoint)
        {
            PointManager pointManager = new PointManager(successIncreasePoint, failPointDamage);
            CardManager cardManager = new CardManager(pointManager);
            cardManager.StarGame(6, false);

            CurrentPointShouldBe(pointManager, 0);

            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);
            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 6, 8, MatchType.NotMatch);
            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.Match);

            CurrentPointShouldBe(pointManager, expectedFinalPoint);
        }

        [Test]
        //選擇已掀開的牌
        public void select_card_that_has_been_revealed()
        {
            CardManager cardManager = new CardManager();
            cardManager.StarGame(5, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopCardShouldBeWrong(cardManager, 0);
            FlopCardShouldBeWrong(cardManager, 3);
            FlopCardShouldBeFirst(cardManager, 4);
            FlopCardShouldBeWrong(cardManager, 4);
        }

        private void FlopCardShouldBeFirst(CardManager cardManager, int cardNumber)
        {
            cardManager.Flop(cardNumber, out MatchType matchResultType);
            Assert.AreEqual(MatchType.None, matchResultType);
        }

        private void FlopCardShouldBeWrong(CardManager cardManager, int cardNumber)
        {
            cardManager.Flop(cardNumber, out MatchType matchResultType);
            Assert.AreEqual(MatchType.WrongSelect, matchResultType);
        }

        private void CurrentPointShouldBe(PointManager pointManager, int expectedCurrentPoint)
        {
            Assert.AreEqual(expectedCurrentPoint, pointManager.GetPoint);
        }

        private void FlopTwoCardResultShouldBe(CardManager cardManager, int card1Number, int card2Number, MatchType expectedMatchType)
        {
            cardManager.Flop(card1Number, out MatchType matchResultType);
            Assert.AreEqual(MatchType.None, matchResultType);
            cardManager.Flop(card2Number, out matchResultType);
            Assert.AreEqual(expectedMatchType, matchResultType);
        }

        private void CoveredCardCountShouldBe(CardManager cardManager, int expectedCount)
        {
            Assert.AreEqual(expectedCount, cardManager.GetTotalCoveredCardCount);
        }

        private void AllCardsShouldBePair(CardManager cardManager)
        {
            Dictionary<int, int> cardPatternDict = new Dictionary<int, int>();
            foreach (Card card in cardManager.GetAllCards)
            {
                if (cardPatternDict.ContainsKey(card.GetPattern))
                    cardPatternDict[card.GetPattern] += 1;
                else
                    cardPatternDict[card.GetPattern] = 1;
            }

            foreach (int patternCount in cardPatternDict.Values)
            {
                Assert.AreEqual(2, patternCount);
            }
        }
    }
}