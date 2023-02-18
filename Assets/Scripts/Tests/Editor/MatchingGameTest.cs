using System.Collections.Generic;
using NUnit.Framework;

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
        CardManager cardManager = new CardManager();
        cardManager.StarGame(4, false);

        PointManager pointManager = new PointManager();
        Assert.AreEqual(0, pointManager.GetPoint);
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
        Dictionary<int, int> dict_cardPattern = new Dictionary<int, int>();
        foreach (Card card in cardManager.GetAllCards)
        {
            if (dict_cardPattern.ContainsKey(card.GetPattern))
                dict_cardPattern[card.GetPattern] += 1;
            else
                dict_cardPattern[card.GetPattern] = 1;
        }

        foreach (int patternCount in dict_cardPattern.Values)
        {
            Assert.AreEqual(2, patternCount);
        }
    }
}