public class Card
{
    public bool IsCovered { get; }
    public int GetPattern { get; }

    public Card(int patternNumber)
    {
        this.GetPattern = patternNumber;
        IsCovered = true;
    }
}