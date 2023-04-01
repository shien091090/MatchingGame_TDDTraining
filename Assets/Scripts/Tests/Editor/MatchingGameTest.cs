using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace GameCore
{
    public class MatchingGameTest
    {
        private Mock<IPatternSetting> patternSettingMock;

        [SetUp]
        public void Setup()
        {
            SetupPatternSettingMock();
        }

        [Test]
        [TestCase(3, 6)]
        [TestCase(7, 14)]
        //遊戲開始, 場上有N*N覆蓋的牌, 且每種牌兩兩一組
        public void game_start_and_all_card_covered(int pairCount, int expectedCount)
        {
            CardManager cardManager = new CardManager(patternSettingMock.Object);
            cardManager.StarGame(pairCount);

            CoveredCardCountShouldBe(cardManager, expectedCount);
            AllCardsShouldBePair(cardManager);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌不同
        public void flop_two_card_and_not_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = new CardManager(patternSettingMock.Object);
            cardManager.StarGame(8, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 2, MatchType.NotMatch);
            CoveredCardCountShouldBe(cardManager, 16);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌相同
        public void flop_two_card_and_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = new CardManager(patternSettingMock.Object);
            cardManager.StarGame(4, false);

            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.Match);
            CoveredCardCountShouldBe(cardManager, 6);
        }

        [Test]
        //遊戲開始, 尚未掀牌, 積分為0
        public void start_game_and_point_is_zero()
        {
            PointManager pointManager = new PointManager();
            CardManager cardManager = new CardManager(patternSettingMock.Object, pointManager);
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
            CardManager cardManager = new CardManager(patternSettingMock.Object, pointManager);
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
            CardManager cardManager = new CardManager(patternSettingMock.Object, pointManager);
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
        //選擇牌之後非成功或失敗的結果, 不影響積分
        public void flop_not_account_point_card()
        {
            PointManager pointManager = new PointManager(5, 2);
            CardManager cardManager = new CardManager(patternSettingMock.Object, pointManager);
            cardManager.StarGame(6, false);

            CurrentPointShouldBe(pointManager, 0);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            CurrentPointShouldBe(pointManager, 5);

            FlopCardShouldBeFirst(cardManager, 2);
            CurrentPointShouldBe(pointManager, 5);

            FlopCardShouldBeWrong(cardManager, 2);
            CurrentPointShouldBe(pointManager, 5);

            FlopCardShouldBeWrong(cardManager, 1);
            CurrentPointShouldBe(pointManager, 5);

            FlopSecondCardShouldBe(cardManager, 4, MatchType.NotMatch);
            CurrentPointShouldBe(pointManager, 3);
        }

        [Test]
        //選擇已掀開的牌
        public void select_card_that_has_been_revealed()
        {
            CardManager cardManager = new CardManager(patternSettingMock.Object);
            cardManager.StarGame(5, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopCardShouldBeWrong(cardManager, 0);
            FlopCardShouldBeWrong(cardManager, 3);
            FlopCardShouldBeFirst(cardManager, 4);
            FlopCardShouldBeWrong(cardManager, 4);
        }

        [Test]
        //場上剩下最後兩張牌, 翻開兩張牌, 遊戲結束
        public void flop_last_two_covered_card_then_game_finish()
        {
            CardManager cardManager = new CardManager(patternSettingMock.Object);
            cardManager.StarGame(3, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.MatchAndGameFinish);
        }

        [Test]
        //遊戲結束後, 重新開始遊戲
        public void game_complete_then_restart()
        {
            PointManager pointManager = new PointManager(2, 1);
            CardManager cardManager = new CardManager(patternSettingMock.Object, pointManager);
            cardManager.StarGame(1, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.MatchAndGameFinish);
            CurrentPointShouldBe(pointManager, 2);

            cardManager.RestartGame();
            CoveredCardCountShouldBe(cardManager, 2);
            CurrentPointShouldBe(pointManager, 0);
        }

        private void FlopSecondCardShouldBe(CardManager cardManager, int cardNumber, MatchType expectedMatchType)
        {
            cardManager.Flop(cardNumber, out MatchType matchResult);
            Assert.AreEqual(expectedMatchType, matchResult);
        }

        private void FlopCardShouldBeFirst(CardManager cardManager, int cardNumber)
        {
            cardManager.Flop(cardNumber, out MatchType matchResultType);
            Assert.AreEqual(MatchType.WaitForNextCard, matchResultType);
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
            Assert.AreEqual(MatchType.WaitForNextCard, matchResultType);
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

        private void SetupPatternSettingMock()
        {
            patternSettingMock = new Mock<IPatternSetting>();
            patternSettingMock.Setup(x => x.GetPatternNumberList()).Returns(new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
        }
    }
}