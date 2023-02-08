public class Card
{
    private int patternNumber;
    public bool IsCovered { get; }

    public Card(int patternNumber)
    {
        this.patternNumber = patternNumber;
        IsCovered = true;
    }
}