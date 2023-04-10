using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SNShien.Common.MockTools;
using SNShien.Common.TimeTools;

namespace GameCore
{
    public class MatchingGameTest
    {
        private Mock<IPatternSetting> patternSettingMock;
        private Mock<IGameSetting> gameSettingMock;
        private ArchitectureEventMock gameEventMock;
        private ArchitectureEventMock presetnterEventMock;

        [SetUp]
        public void Setup()
        {
            SetupPatternSettingMock();
            SetupGameSettingMock();
            gameEventMock = new ArchitectureEventMock();
        }

        [Test]
        [TestCase(3, 6)]
        [TestCase(7, 14)]
        //遊戲開始, 場上有N*N覆蓋的牌, 且每種牌兩兩一組
        public void game_start_and_all_card_covered(int pairCount, int expectedCount)
        {
            CardManager cardManager = GivenCardManagerAndStartGame(pairCount);

            CoveredCardCountShouldBe(cardManager, expectedCount);
            AllCardsShouldBePair(cardManager);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌不同
        public void flop_two_card_and_not_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = GivenCardManagerAndStartGame(8, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 2, MatchType.NotMatch);
            CoveredCardCountShouldBe(cardManager, 16);
        }

        [Test]
        //場上所有牌仍是覆蓋狀態, 翻開兩張牌, 兩張牌相同
        public void flop_two_card_and_same_pattern_when_all_cards_covered()
        {
            CardManager cardManager = GivenCardManagerAndStartGame(4, false);

            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.Match);
            CoveredCardCountShouldBe(cardManager, 6);
        }

        [Test]
        //遊戲開始, 尚未掀牌, 積分為0
        public void start_game_and_point_is_zero()
        {
            PointManager pointManager = GivenPointManager();
            CardManager cardManager = GivenCardManagerAndStartGame(4, false, pointManager);

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
            PointManager pointManager = GivenPointManager(successIncreasePoint, failPointDamage);
            CardManager cardManager = GivenCardManagerAndStartGame(6, false, pointManager);

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
            PointManager pointManager = GivenPointManager(successIncreasePoint, failPointDamage);
            CardManager cardManager = GivenCardManagerAndStartGame(6, false, pointManager);

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
            PointManager pointManager = GivenPointManager(5, 2);
            CardManager cardManager = GivenCardManagerAndStartGame(6, false, pointManager);

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
            CardManager cardManager = GivenCardManagerAndStartGame(5, false);

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
            CardManager cardManager = GivenCardManagerAndStartGame(3, false);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 2, 3, MatchType.Match);
            FlopTwoCardResultShouldBe(cardManager, 4, 5, MatchType.MatchAndGameFinish);
        }

        [Test]
        //遊戲結束後, 重新開始遊戲
        public void game_complete_then_restart()
        {
            PointManager pointManager = GivenPointManager(2, 1);
            CardManager cardManager = GivenCardManagerAndStartGame(1, false, pointManager);

            FlopTwoCardResultShouldBe(cardManager, 0, 1, MatchType.MatchAndGameFinish);
            CurrentPointShouldBe(pointManager, 2);

            cardManager.RestartGame();
            CoveredCardCountShouldBe(cardManager, 2);
            CurrentPointShouldBe(pointManager, 0);
        }

        private PointManager GivenPointManager(int successIncreasePoint = 0, int failPointDamage = 0)
        {
            PointManager pointManager = new PointManager(successIncreasePoint, failPointDamage);
            pointManager.Construct(gameEventMock.GetEventInvoker);
            return pointManager;
        }

        private CardManager GivenCardManagerAndStartGame(int pairCount, bool useShuffle = true, PointManager pointManager = null)
        {
            CardManager cardManager = new CardManager(
                patternSettingMock.Object,
                gameEventMock.GetEventInvoker,
                gameSettingMock.Object,
                new TimeAsyncExecuter(),
                pointManager);
            
            cardManager.StartGame(pairCount, useShuffle);
            gameEventMock.VerifyEventTriggerTimes<StartGameEvent>(1);

            StartGameEvent startGameEventInfo = gameEventMock.GetLastTriggerEventInfo<StartGameEvent>();
            CardPresenter cardPresenter = startGameEventInfo.CardPresenter;
            presetnterEventMock = new ArchitectureEventMock();
            cardPresenter.SetEventInvoker(presetnterEventMock.GetEventInvoker);

            return cardManager;
        }

        private void FlopSecondCardShouldBe(CardManager cardManager, int cardNumber, MatchType expectedMatchType)
        {
            gameEventMock.ClearEventRecord<FlopCardEvent>();
            presetnterEventMock.ClearEventRecord<SwitchCoverStateEvent>();
            presetnterEventMock.ClearEventRecord<PlayCardMatchEffectEvent>();
            presetnterEventMock.ClearEventRecord<RefreshButtonFrozeStateEvent>();

            cardManager.Flop(cardNumber, out MatchType matchResult);
            Assert.AreEqual(expectedMatchType, matchResult);

            FlopCardEvent flopCardEvent = gameEventMock.GetLastTriggerEventInfo<FlopCardEvent>();
            Assert.AreEqual(expectedMatchType, flopCardEvent.MatchResult);
            
            presetnterEventMock.VerifyEventTriggerTimes<RefreshButtonFrozeStateEvent>(2);
            List<RefreshButtonFrozeStateEvent> refreshButtonFrozeStateEvents = presetnterEventMock.GetTriggerEventInfoList<RefreshButtonFrozeStateEvent>();
            Assert.IsTrue(refreshButtonFrozeStateEvents[0].IsFroze);
            Assert.IsFalse(refreshButtonFrozeStateEvents[1].IsFroze);

            List<SwitchCoverStateEvent> triggerEventInfoList;
            switch (flopCardEvent.MatchResult)
            {
                case MatchType.Match:
                case MatchType.MatchAndGameFinish:
                    presetnterEventMock.VerifyEventTriggerTimes<SwitchCoverStateEvent>(1);
                    triggerEventInfoList = presetnterEventMock.GetTriggerEventInfoList<SwitchCoverStateEvent>();
                    Assert.IsNotNull(triggerEventInfoList.FirstOrDefault(x => x.CardNumber == cardNumber && x.IsCover == false));

                    presetnterEventMock.VerifyEventTriggerTimes<PlayCardMatchEffectEvent>(1);
                    PlayCardMatchEffectEvent playCardMatchEffectEventInfo = presetnterEventMock.GetLastTriggerEventInfo<PlayCardMatchEffectEvent>();
                    Assert.IsTrue(playCardMatchEffectEventInfo.CheckIsMatchNumber(cardNumber));
                    break;

                case MatchType.NotMatch:
                    presetnterEventMock.VerifyEventTriggerTimes<SwitchCoverStateEvent>(3);
                    triggerEventInfoList = presetnterEventMock.GetTriggerEventInfoList<SwitchCoverStateEvent>();
                    Assert.IsNotNull(triggerEventInfoList.FirstOrDefault(x => x.CardNumber == cardNumber && x.IsCover));
                    Assert.IsNotNull(triggerEventInfoList.FirstOrDefault(x => x.CardNumber == cardNumber && x.IsCover == false));
                    break;
            }
        }

        private void FlopCardShouldBeFirst(CardManager cardManager, int cardNumber)
        {
            gameEventMock.ClearEventRecord<FlopCardEvent>();
            presetnterEventMock.ClearEventRecord<SwitchCoverStateEvent>();
            presetnterEventMock.ClearEventRecord<RefreshButtonFrozeStateEvent>();

            cardManager.Flop(cardNumber, out MatchType matchResultType);
            Assert.AreEqual(MatchType.WaitForNextCard, matchResultType);
            gameEventMock.VerifyEventTriggerTimes<FlopCardEvent>(1);
            presetnterEventMock.VerifyEventTriggerTimes<SwitchCoverStateEvent>(1);
            presetnterEventMock.VerifyEventTriggerTimes<RefreshButtonFrozeStateEvent>(2);

            FlopCardEvent flopCardEvent = gameEventMock.GetLastTriggerEventInfo<FlopCardEvent>();
            Assert.AreEqual(MatchType.WaitForNextCard, flopCardEvent.MatchResult);

            SwitchCoverStateEvent lastTriggerEventInfo = presetnterEventMock.GetLastTriggerEventInfo<SwitchCoverStateEvent>();
            Assert.AreEqual(cardNumber, lastTriggerEventInfo.CardNumber);
            Assert.IsFalse(lastTriggerEventInfo.IsCover);

            List<RefreshButtonFrozeStateEvent> refreshButtonFrozeStateEvents = presetnterEventMock.GetTriggerEventInfoList<RefreshButtonFrozeStateEvent>();
            Assert.IsTrue(refreshButtonFrozeStateEvents[0].IsFroze);
            Assert.IsFalse(refreshButtonFrozeStateEvents[1].IsFroze);
        }

        private void FlopCardShouldBeWrong(CardManager cardManager, int cardNumber)
        {
            gameEventMock.ClearEventRecord<FlopCardEvent>();
            cardManager.Flop(cardNumber, out MatchType matchResultType);
            gameEventMock.VerifyEventTriggerTimes<FlopCardEvent>(0);
            Assert.AreEqual(MatchType.WrongSelect, matchResultType);
        }

        private void CurrentPointShouldBe(PointManager pointManager, int expectedCurrentPoint)
        {
            Assert.AreEqual(expectedCurrentPoint, pointManager.GetPoint);
        }

        private void FlopTwoCardResultShouldBe(CardManager cardManager, int card1Number, int card2Number, MatchType expectedMatchType)
        {
            FlopCardShouldBeFirst(cardManager, card1Number);
            FlopSecondCardShouldBe(cardManager, card2Number, expectedMatchType);
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

        private void SetupGameSettingMock()
        {
            gameSettingMock = new Mock<IGameSetting>();
            gameSettingMock.Setup(x => x.GetCardDelayCoverTimes).Returns(1);
        }

        private void SetupPatternSettingMock()
        {
            patternSettingMock = new Mock<IPatternSetting>();
            patternSettingMock.Setup(x => x.GetPatternNumberList()).Returns(new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
        }
    }
}