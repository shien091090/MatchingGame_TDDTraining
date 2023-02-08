using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json.Serialization;

public class MatchingGameTest
{
    [Test]
    public void game_start_and_all_card_covered()
    {
        CardManager cardManager = new CardManager();
        cardManager.StarGame(3);
        Assert.AreEqual(6,cardManager.GetTotalCoveredCardCount);
    }
}