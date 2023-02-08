using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager
{
    private List<int> patternPool;
    private List<Card> cardList;

    public int GetTotalCoveredCardCount => cardList.Count(x => x.IsCovered);

    public CardManager()
    {
        patternPool = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
    }

    private int GetRandomPatternNumber()
    {
        int randomIndex = Random.Range(0, patternPool.Count);
        int patternNumber = patternPool[randomIndex];
        patternPool.RemoveAt(randomIndex);
        return patternNumber;
    }

    public void StarGame(int pairCount)
    {
        cardList = new List<Card>();
        
        for (int i = 0; i < pairCount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int patternNumber = GetRandomPatternNumber();
                Card card = new Card(patternNumber);
                cardList.Add(card);
            }
        }
    }
}