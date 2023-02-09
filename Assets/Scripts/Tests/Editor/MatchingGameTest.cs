using System.Collections.Generic;
using NUnit.Framework;

public class MatchingGameTest
{
    [Test]
    [TestCase(3, 6)]
    [TestCase(7, 14)]
    public void game_start_and_all_card_covered(int pairCount, int expectedCount)
    {
        CardManager cardManager = new CardManager();
        cardManager.StarGame(pairCount);

        CardCountShouldBe(cardManager, expectedCount);
        AllCardsShouldBePair(cardManager);
    }

    private void CardCountShouldBe(CardManager cardManager, int expectedCount)
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